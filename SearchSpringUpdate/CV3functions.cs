using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using CV3ServiceProxy.Services;

using static SearchSpringUpdate.DatabaseFunctions;

using log4net;

namespace SearchSpringUpdate
{
    public static class CV3functions
    {
        //static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get a list of all Product objects from CV3
        /// </summary>
        /// <param name="store"></param>
        /// <param name="categories">Out parameter with list of all unique categories (this should probably be seperate, we need the authentication/client for this, so this seemed best)</param>
        /// <param name="azureSqlCon"></param>
        /// <returns></returns>
        public static List<Product> getCV3Products(string store, out List<Category> categories, SqlConnection azureSqlCon)
        {
            EndpointAddress serviceURL;
            string authentication = getAuthenticationXml(store, azureSqlCon, out serviceURL);

            BasicHttpBinding wcfBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);

            wcfBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            wcfBinding.ReaderQuotas.MaxDepth = 2147483647;
            wcfBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
            wcfBinding.ReaderQuotas.MaxArrayLength = 2147483647;
            wcfBinding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            wcfBinding.ReaderQuotas.MaxNameTableCharCount = 2147483647;

            wcfBinding.MaxReceivedMessageSize = 2147483647;
            wcfBinding.MaxBufferSize = 2147483647;
            wcfBinding.MaxBufferPoolSize = 2147483647;


            CV3DataxsdPortTypeClient client = new CV3DataxsdPortTypeClient(wcfBinding, serviceURL);

            List<XElement> productIDs = getProductIDs(authentication, client);
            //log.Debug($"Found {productIDs.Count} product IDs.");
            List<List<XElement>> productIDsChunked = getChunks(productIDs, 500);
            List<Product> productList = new List<Product>();
            for (int i = 0; i < productIDsChunked.Count; i++)
            {
                //log.Debug($"Getting products {productIDsChunked[i].First()} through {productIDsChunked[i].Last()}");
                productList.AddRange(getProductRange(productIDsChunked[i].First().Value, productIDsChunked[i].Last().Value, authentication, client));
            }
            categories = getAllCategoriesFromProducts(productList, authentication, client);
            return productList;
        }

        /// <summary>
        /// Get the IDs of all products from CV3
        /// </summary>
        /// <param name="authentication"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static List<XElement> getProductIDs(string authentication, CV3DataxsdPortTypeClient client)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<CV3Data version=\"2.0\">" +
                    "<request>" + authentication +
                        "<requests>" +
                            "<reqProductIDs/>" +
                        "</requests>" +
                    "</request>" +
                "</CV3Data>";
            string responseStr = Encoding.UTF8.GetString(client.CV3Data(convertToBase64(xml)));
            //TODO: Check for errors
            XDocument productIDsXML = XDocument.Parse(responseStr);
            return productIDsXML.Descendants("ID").ToList<XElement>();
        }

        /// <summary>
        /// Get all products in a range from Start to End. Used in GetCV3Products because the data must be retreived in chunks.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="authentication"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static List<Product> getProductRange(string start, string end, string authentication, CV3DataxsdPortTypeClient client)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<CV3Data version=\"2.0\">" +
                    "<request>" + authentication +
                        "<requests>" +
                            "<reqProducts>" +
                                "<reqProductRange start=\"" + start + "\" end=\"" + end + "\" />" +
                            "</reqProducts>" +
                        "</requests>" +
                    "</request>" +
                "</CV3Data>";
            string responseStr = Encoding.UTF8.GetString(client.CV3Data(convertToBase64(xml)));
            //File.WriteAllText("C:\\Test\\cv3.xml", responseStr);
            //TODO: Check for errors
            XmlSerializer serializer = new XmlSerializer(typeof(CV3Data));
            StringReader reader = new StringReader(responseStr);
            CV3Data data = (CV3Data)serializer.Deserialize(reader);
            return data.Products.ProductList;
        }

        /// <summary>
        /// Loop through the given list of products and get the unique categories.
        /// </summary>
        /// <param name="products"></param>
        /// <param name="authentication"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static List<Category> getAllCategoriesFromProducts(List<Product> products, string authentication, CV3DataxsdPortTypeClient client)
        {
            List<string> idList = new List<string>();
            string thisCategoryID;
            for(int i = 0; i < products.Count; i++)
            {
                for(int j = 0; j < products[i].Categories.ID.Count; j++)
                {
                    thisCategoryID = products[i].Categories.ID[j];
                    if (!idList.Exists(x => x == thisCategoryID))
                    {
                        idList.Add(thisCategoryID);
                    }
                }
            }
            idList.Sort();
            List<Category> outList = new List<Category>();
            List<List<string>> idListChunked = getChunks(idList, 500);
            List<string> thisChunk;
            List<Category> tempCategoryList;
            for(int i = 0; i < idListChunked.Count; i++)
            {
                thisChunk = idListChunked[i];
                //log.Debug($"Getting categories {thisChunk.First()} through {thisChunk.Last()}");
                tempCategoryList = getCategoryRange(thisChunk.First(), thisChunk.Last(), authentication, client);
                for (int j = 0; j < tempCategoryList.Count; j++)
                {
                    if (thisChunk.Contains(tempCategoryList[j].ID))
                    {
                        outList.Add(tempCategoryList[j]);
                    }
                }
            }

            return outList;
        }

        /// <summary>
        /// Gets a Category object from CV3 for the given categoryID.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="authentication"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Category getCategoryByID(string categoryID, string authentication, CV3DataxsdPortTypeClient client)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<CV3Data version=\"2.0\">" +
                    "<request>" + authentication +
                        "<requests>" +
                            "<reqCategories>" +
                                "<reqCategorySingle>" + categoryID + "</reqCategorySingle>" +
                            "</reqCategories>" +
                        "</requests>" +
                    "</request>" +
                "</CV3Data>";

            string responseStr = Encoding.UTF8.GetString(client.CV3Data(convertToBase64(xml)));
            //File.WriteAllText("C:\\Test\\cv3.xml", responseStr);
            //TODO: Check for errors
            XmlSerializer serializer = new XmlSerializer(typeof(CV3Data));
            StringReader reader = new StringReader(responseStr);
            CV3Data data = (CV3Data)serializer.Deserialize(reader);
            Category outCategory = data.categories.category[0];
            return outCategory;
        }

        /// <summary>
        /// Get all categories in a range.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="authentication"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static List<Category> getCategoryRange(string start, string end, string authentication, CV3DataxsdPortTypeClient client)
        {
            try
            {
                string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<CV3Data version=\"2.0\">" +
                        "<request>" + authentication +
                            "<requests>" +
                                "<reqCategories>" +
                                    $"<reqCategoryRange start='{start}' end='{end}' />" +
                                "</reqCategories>" +
                            "</requests>" +
                        "</request>" +
                    "</CV3Data>";

                string responseStr = Encoding.UTF8.GetString(client.CV3Data(convertToBase64(xml)));
                //File.WriteAllText("C:\\Test\\cv3.xml", responseStr);
                //TODO: Check for errors
                XmlSerializer serializer = new XmlSerializer(typeof(CV3Data));
                StringReader reader = new StringReader(responseStr);
                CV3Data data = (CV3Data)serializer.Deserialize(reader);
                List<Category> outCategories = data.categories.category;
                return outCategories;
            }
            catch(Exception ex)
            {
                //log.Error("Error in getCategoryRange", ex);
                return null;
            }
        }

        /// <summary>
        /// Break a list of XElements into chunks of the given size.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        static List<List<XElement>> getChunks(List<XElement> list, int chunkSize)
        {
            List<List<XElement>> outList = new List<List<XElement>>();
            if (list.Count > chunkSize)
            {
                List<XElement> chunk = new List<XElement>();
                for (int i = 0; i < list.Count; i++)
                {
                    chunk.Add(list[i]);
                    if (i != 0 && i % chunkSize == 0)
                    {
                        outList.Add(chunk);
                        chunk = new List<XElement>();
                    }
                    //if (i > 1500) { return outList; }
                }
            }
            else
            {
                outList.Add(list);
            }
            return outList;
        }

        /// <summary>
        /// Break a list of strings into chunks of the given size.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        static List<List<string>> getChunks(List<string> list, int chunkSize)
        {
            List<List<string>> outList = new List<List<string>>();
            if (list.Count > chunkSize)
            {
                List<string> chunk = new List<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    chunk.Add(list[i]);
                    if (i != 0 && i % chunkSize == 0)
                    {
                        outList.Add(chunk);
                        chunk = new List<string>();
                    }
                    //if (i > 1500) { return outList; }
                }
            }
            else
            {
                outList.Add(list);
            }
            return outList;
        }

        /// <summary>
        /// Converts text into Base64 text, which is what CV3 expects for its XML messages.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string convertToBase64(string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            string base64text = Convert.ToBase64String(textBytes);
            return base64text;
        }
    }
}

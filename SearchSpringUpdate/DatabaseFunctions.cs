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
using System.Text.RegularExpressions;
using System.IO.Compression;

using CV3ServiceProxy.Services;
using CsvHelper;
using log4net;

namespace SearchSpringUpdate
{
    public static class DatabaseFunctions
    {
        //static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Wipes the SearchSpringProducts table, then inserts the provided list. Returns the list with ourID added.
        /// </summary>
        /// <param name="products"></param>
        /// <param name="store"></param>
        /// <param name="integrationSqlCon"></param>
        /// <returns></returns>
        public static List<Product> insertCV3Products(List<Product> products, int store, SqlConnection integrationSqlCon)
        {
            //log.Debug("Wiping SearchSpringProducts");
            SqlCommand productTruncateCommand = new SqlCommand("TRUNCATE TABLE SearchSpringProducts", integrationSqlCon);
            productTruncateCommand.ExecuteNonQuery();

            //log.Debug("Writing products to database");

            DataTable SearchSpringProducts = new DataTable();
            SearchSpringProducts.Columns.Add("id");
            SearchSpringProducts.Columns.Add("sku");
            SearchSpringProducts.Columns.Add("cv3id", typeof(int));
            SearchSpringProducts.Columns.Add("name");
            SearchSpringProducts.Columns.Add("manufacturer");
            SearchSpringProducts.Columns.Add("brand");
            SearchSpringProducts.Columns.Add("retailPrice");
            SearchSpringProducts.Columns.Add("specialPrice");
            SearchSpringProducts.Columns.Add("defaultCategoryID");
            SearchSpringProducts.Columns.Add("description");
            SearchSpringProducts.Columns.Add("store", typeof(int));
            SearchSpringProducts.Columns.Add("isInactive");
            SearchSpringProducts.Columns.Add("store1Inventory");
            SearchSpringProducts.Columns.Add("store3Inventory");
            SearchSpringProducts.Columns.Add("locations");
            SearchSpringProducts.Columns.Add("popularity");
            SearchSpringProducts.Columns.Add("keywords");

            DataRow CV3row;
            Product thisProd;
            Custom thisCustom;
            for (int i = 0; i < products.Count; i++)
            {
                thisProd = products[i];
                if (thisProd.Inactive == "false")
                {
                    products[i].ourID = i;
                    CV3row = SearchSpringProducts.NewRow();
                    CV3row["id"] = i;
                    CV3row["sku"] = thisProd.SKU;
                    CV3row["cv3id"] = Convert.ToInt32(thisProd.ProdID);
                    CV3row["name"] = thisProd.Name;
                    CV3row["manufacturer"] = thisProd.Manufacturer;
                    CV3row["brand"] = thisProd.Brand;
                    if (thisProd.Retail != null && thisProd.Retail.Price != null)
                    {
                        if (thisProd.Retail.Price.StandardPrice != null)
                        {
                            CV3row["retailPrice"] = thisProd.Retail.Price.StandardPrice;
                        }
                        if (thisProd.Retail.Price.SpecialPrice != null)
                        {
                            CV3row["specialPrice"] = thisProd.Retail.Price.SpecialPrice;
                        }
                    }
                    CV3row["defaultCategoryID"] = thisProd.DefaultCategory;
                    CV3row["description"] = thisProd.Description;
                    CV3row["store"] = store;
                    CV3row["isInactive"] = thisProd.Inactive;
                    CV3row["keywords"] = thisProd.Keywords;

                    if (thisProd.Custom != null && thisProd.Custom.Count > 0)
                    {
                        for (int j = 0; j < thisProd.Custom.Count; j++)
                        {
                            thisCustom = thisProd.Custom[j];
                            switch (thisCustom.Id)
                            {
                                case "10":
                                    CV3row["locations"] = thisCustom.Text;
                                    break;
                                case "12":
                                    CV3row["store1Inventory"] = thisCustom.Text;
                                    break;
                                case "13":
                                    CV3row["store3Inventory"] = thisCustom.Text;
                                    break;
                                case "14":
                                    CV3row["popularity"] = thisCustom.Text;
                                    break;
                            }
                        }
                    }

                    SearchSpringProducts.Rows.Add(CV3row);
                }
            }

            SqlBulkCopy productBulkCopy = new SqlBulkCopy(integrationSqlCon);
            productBulkCopy.DestinationTableName = "SearchSpringProducts";
            productBulkCopy.WriteToServer(SearchSpringProducts);
            return products;
        }

        /// <summary>
        /// Wipes the SearchSpringAttribute table, then finds the unique attributes from the products list and inserts them to SearchSpringAttribute.
        /// </summary>
        /// <param name="products"></param>
        /// <param name="store"></param>
        /// <param name="integrationSqlCon"></param>
        /// <returns></returns>
        public static string insertCV3Attributes(List<Product> products, int store, SqlConnection integrationSqlCon)
        {
            //log.Debug("Wiping SearchSpringAttribute");

            SqlCommand attributeTruncateCommand = new SqlCommand("TRUNCATE TABLE SearchSpringAttribute", integrationSqlCon);
            attributeTruncateCommand.ExecuteNonQuery();

            //log.Debug("Writing attributes to database");

            DataTable SearchSpringAttributes = new DataTable();
            SearchSpringAttributes.Columns.Add("id");
            SearchSpringAttributes.Columns.Add("ourID");
            SearchSpringAttributes.Columns.Add("parentSKU");
            SearchSpringAttributes.Columns.Add("thisSKU");
            SearchSpringAttributes.Columns.Add("altID");
            SearchSpringAttributes.Columns.Add("title");
            SearchSpringAttributes.Columns.Add("attribute");
            SearchSpringAttributes.Columns.Add("code");
            SearchSpringAttributes.Columns.Add("retailPrice");
            SearchSpringAttributes.Columns.Add("specialPrice");
            SearchSpringAttributes.Columns.Add("status");

            DataRow CV3row;
            Product thisProd;
            Attribute thisAttribute;
            for (int i = 0; i < products.Count; i++)
            {
                thisProd = products[i];
                if (thisProd.Inactive == "false")
                {
                    for (int j = 0; j < thisProd.Attributes.Attribute.Count; j++)
                    {
                        thisAttribute = thisProd.Attributes.Attribute[j];
                        CV3row = SearchSpringAttributes.NewRow();
                        CV3row["ourID"] = thisProd.ourID;
                        CV3row["parentSKU"] = thisProd.SKU;
                        CV3row["thisSKU"] = thisAttribute.SKU;
                        CV3row["altID"] = thisAttribute.AltID;
                        CV3row["title"] = thisProd.Attributes.Titles.Title[0].Text;
                        CV3row["attribute"] = thisAttribute.Combination.Value[0].Text;
                        CV3row["code"] = thisAttribute.Codes.Value[0].Text;
                        CV3row["retailPrice"] = Convert.ToDecimal(thisAttribute.Retail.Price.StandardPrice);
                        CV3row["specialPrice"] = Convert.ToDecimal(thisAttribute.Retail.Price.SpecialPrice);
                        CV3row["status"] = "active";

                        SearchSpringAttributes.Rows.Add(CV3row);
                    }
                }
            }

            SqlBulkCopy attributeBulkCopy = new SqlBulkCopy(integrationSqlCon);
            attributeBulkCopy.DestinationTableName = "SearchSpringAttribute";
            attributeBulkCopy.WriteToServer(SearchSpringAttributes);
            return "Success";
        }

        /// <summary>
        /// Wipes the SearchSpringImageSet table, then takes the images from the products list and inserts them to SearchSpringImageSet.
        /// </summary>
        /// <param name="products"></param>
        /// <param name="store"></param>
        /// <param name="integrationSqlCon"></param>
        /// <returns></returns>
        public static string insertCV3ImageSet(List<Product> products, int store, SqlConnection integrationSqlCon)
        {
            //log.Debug("Wiping SearchSpringImageSet");

            SqlCommand imageSetTruncateCommand = new SqlCommand("TRUNCATE TABLE SearchSpringImageSet", integrationSqlCon);
            imageSetTruncateCommand.ExecuteNonQuery();

            //log.Debug("Writing images to database");

            DataTable SearchSpringImageSet = new DataTable();
            SearchSpringImageSet.Columns.Add("id");
            SearchSpringImageSet.Columns.Add("ourID");
            SearchSpringImageSet.Columns.Add("thumb");
            SearchSpringImageSet.Columns.Add("larg");
            SearchSpringImageSet.Columns.Add("popup");
            SearchSpringImageSet.Columns.Add("type");
            SearchSpringImageSet.Columns.Add("title");
            SearchSpringImageSet.Columns.Add("attributeName");
            SearchSpringImageSet.Columns.Add("setInactive");
            SearchSpringImageSet.Columns.Add("setRank");

            DataRow CV3row;
            Product thisProd;
            Image thisImage;
            for (int i = 0; i < products.Count; i++)
            {
                thisProd = products[i];
                if (thisProd.Inactive == "false")
                {
                    for (int j = 0; j < thisProd.Images.Image.Count; j++)
                    {
                        thisImage = thisProd.Images.Image[j];
                        if (thisImage != null)
                        {
                            CV3row = SearchSpringImageSet.NewRow();
                            CV3row["ourID"] = thisProd.ourID;
                            CV3row["thumb"] = thisImage.Thumbnail;
                            CV3row["larg"] = thisImage.Large;
                            CV3row["popup"] = thisImage.PopUp;
                            CV3row["type"] = thisImage.Type;
                            CV3row["title"] = thisImage.Title;
                            CV3row["attributeName"] = thisImage.Title;
                            CV3row["setInactive"] = thisImage.Inactive;
                            CV3row["setRank"] = 1;

                            SearchSpringImageSet.Rows.Add(CV3row);
                        }
                    }
                }
            }

            SqlBulkCopy imageSetBulkCopy = new SqlBulkCopy(integrationSqlCon);
            imageSetBulkCopy.DestinationTableName = "SearchSpringImageSet";
            imageSetBulkCopy.WriteToServer(SearchSpringImageSet);
            return "Success";
        }

        /// <summary>
        /// Wipes the SearchSpringCategories table, then finds reinserts the new categories to SearchSpringCategories.
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="store"></param>
        /// <param name="integrationSqlCon"></param>
        /// <returns></returns>
        public static string insertCV3Categories(List<Category> categories, int store, SqlConnection integrationSqlCon)
        {
            //log.Debug("Wiping SearchSpringCategories");

            SqlCommand categoryTruncateCommand = new SqlCommand("TRUNCATE TABLE SearchSpringCategories", integrationSqlCon);
            categoryTruncateCommand.ExecuteNonQuery();

            //log.Debug("Writing categories to database");

            DataTable SearchSpringCategories = new DataTable();
            SearchSpringCategories.Columns.Add("CategoryName");
            SearchSpringCategories.Columns.Add("ID");
            SearchSpringCategories.Columns.Add("URLname");

            DataRow CV3row;
            Category thisCategory;
            for (int i = 0; i < categories.Count; i++)
            {
                thisCategory = categories[i];
                CV3row = SearchSpringCategories.NewRow();
                CV3row["CategoryName"] = thisCategory.Name;
                CV3row["ID"] = thisCategory.ID;
                CV3row["URLname"] = thisCategory.URLName;

                SearchSpringCategories.Rows.Add(CV3row);
            }

            SqlBulkCopy CategoriesBulkCopy = new SqlBulkCopy(integrationSqlCon);
            CategoriesBulkCopy.DestinationTableName = "SearchSpringCategories";
            CategoriesBulkCopy.WriteToServer(SearchSpringCategories);
            return "Success";
        }

        /// <summary>
        /// Wipes the SearchSpringFilter, then takes the filters from the products list and inserts them to SearchSpringFilter.
        /// </summary>
        /// <param name="products"></param>
        /// <param name="store"></param>
        /// <param name="integrationSqlCon"></param>
        /// <returns></returns>
        public static string insertCV3Filters(List<Product> products, int store, SqlConnection integrationSqlCon)
        {
            //log.Debug("Wiping SearchSpringFilter");

            SqlCommand filterTruncateCommand = new SqlCommand("TRUNCATE TABLE SearchSpringFilter", integrationSqlCon);
            filterTruncateCommand.ExecuteNonQuery();

            //log.Debug("Writing filters to database");

            DataTable SearchSpringFilter = new DataTable();
            SearchSpringFilter.Columns.Add("id");
            SearchSpringFilter.Columns.Add("ourID");
            SearchSpringFilter.Columns.Add("filter");
            SearchSpringFilter.Columns.Add("value");
            SearchSpringFilter.Columns.Add("sort");

            DataRow CV3row;
            Product thisProd;
            CategoryFilter thisFilter;
            for (int i = 0; i < products.Count; i++)
            {
                thisProd = products[i];
                if (thisProd.Inactive == "false" && thisProd.CategoryFilters != null && thisProd.CategoryFilters.CategoryFilter != null && thisProd.CategoryFilters.CategoryFilter.Count > 0)
                {
                    for (int j = 0; j < thisProd.CategoryFilters.CategoryFilter.Count; j++)
                    {
                        thisFilter = thisProd.CategoryFilters.CategoryFilter[j];
                        if (thisFilter != null)
                        {
                            CV3row = SearchSpringFilter.NewRow();
                            CV3row["ourID"] = thisProd.ourID;
                            CV3row["filter"] = thisFilter.Filter;
                            CV3row["value"] = thisFilter.Value;
                            CV3row["sort"] = thisFilter.SortValue;

                            SearchSpringFilter.Rows.Add(CV3row);
                        }
                    }
                }
            }

            SqlBulkCopy filterSetBulkCopy = new SqlBulkCopy(integrationSqlCon);
            filterSetBulkCopy.DestinationTableName = "SearchSpringFilter";
            filterSetBulkCopy.WriteToServer(SearchSpringFilter);
            return "Success";
        }

        /// <summary>
        /// Wrie the contents of the given MemoryStream object to a file.
        /// </summary>
        /// <param name="memStream"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string writeMemStreamToFile(MemoryStream memStream, string filePath)
        {       //For test

            /*log.Debug("Creating temp directory");
            if (Directory.Exists(outputFolder + tempFolderName))
            {
                Directory.Delete(outputFolder + tempFolderName, true);
            }
            Directory.CreateDirectory(outputFolder + tempFolderName);*/

            //log.Debug("Writing CSV");
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            memStream.WriteTo(fileStream);
            memStream.Flush();
            fileStream.Flush();

            /*log.Debug("Zipping temp directory");
            if (File.Exists(outputFolder + zipName))
            {
                File.Delete(outputFolder + zipName);
            }

            ZipFile.CreateFromDirectory(outputFolder + tempFolderName, outputFolder + zipName);
            log.Debug("Cleaning up");
            if (Directory.Exists(outputFolder + tempFolderName))
            {
                Directory.Delete(outputFolder + tempFolderName, true);
            }*/

            return "Success";
        }

        /// <summary>
        /// Creates a CSV from the vwSearchSpringReport view in memory
        /// </summary>
        /// <param name="integrationSqlCon"></param>
        /// <returns></returns>
        public static MemoryStream createCSVFileStream(SqlConnection integrationSqlCon)
        {

            //log.Debug("Pulling report view");
            SqlCommand reportCommand = new SqlCommand("SELECT * FROM vwSearchSpringReport", integrationSqlCon);
            SqlDataReader reportReader = reportCommand.ExecuteReader();
            DataTable reportTable = new DataTable();
            reportTable.Load(reportReader);
            //log.Debug("Adding attributes");
            addAttributes(reportTable, integrationSqlCon);

            //log.Debug("Creating CSV");
            MemoryStream memStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memStream);
            CsvWriter csv = new CsvWriter(streamWriter);

            foreach (DataColumn column in reportTable.Columns)
            {
                csv.WriteField(column.ColumnName);
            }
            csv.NextRecord();
            foreach (DataRow row in reportTable.Rows)
            {
                for (var i = 0; i < reportTable.Columns.Count; i++)
                {
                    csv.WriteField(row[i]);
                }
                csv.NextRecord();
            }
            streamWriter.Flush();
            memStream.Flush();
            
            return memStream;
        }

        /// <summary>
        /// Takes a file in the form of a MemoryStream and zips it using the given string as the file in memory's name.
        /// </summary>
        /// <param name="memStream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static MemoryStream zipMemoryStream(MemoryStream memStream, string fileName)
        {
            //log.Debug("Zipping");

            MemoryStream outStream = new MemoryStream();
            outStream.Position = 0;
            ZipArchive archive = new ZipArchive(outStream, ZipArchiveMode.Create, true);
            ZipArchiveEntry archiveEntry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
            Stream archiveEntryStream = archiveEntry.Open();
            memStream.WriteTo(archiveEntryStream);
            archiveEntryStream.Flush();
            outStream.Flush();
            archive.Dispose();
            return outStream;
        }

        /*public static string insertCV3Filter(List<Product> products, int store, SqlConnection integrationSqlCon)
        {
            SqlCommand attributeTruncateCommand = new SqlCommand("TRUNCATE TABLE SearchSpringFilter", integrationSqlCon);
            attributeTruncateCommand.ExecuteNonQuery();

            DataTable SearchSpringFilter = new DataTable();
            SearchSpringFilter.Columns.Add("id");
            SearchSpringFilter.Columns.Add("ourID");
            SearchSpringFilter.Columns.Add("filter");
            SearchSpringFilter.Columns.Add("value");
            SearchSpringFilter.Columns.Add("sort");

            DataRow CV3row;
            Product thisProd;
            Attribute thisAttribute;
            for (int i = 0; i < products.Count; i++)
            {
                thisProd = products[i];
                if (thisProd.Inactive == "false")
                {
                    for (int j = 0; j < thisProd.Attributes.Attribute.Count; j++)
                    {
                        SearchSpringFilter.Rows.Add(CV3row);
                    }
                }
            }

            SqlBulkCopy attributeBulkCopy = new SqlBulkCopy(integrationSqlCon);
            attributeBulkCopy.DestinationTableName = "SearchSpringAttribute";
            attributeBulkCopy.WriteToServer(SearchSpringFilter);
            return "Success";
        }*/

        /// <summary>
        /// Build the authentication XML to send to CV3
        /// </summary>
        /// <param name="store">Store 1 or Store 2</param>
        /// <param name="azureSqlCon"></param>
        /// <param name="serviceURL"></param>
        /// <returns></returns>
        public static string getAuthenticationXml(string store, SqlConnection azureSqlCon, out EndpointAddress serviceURL)
        {
            string keyName = "";
            string username = "";
            string password = "";
            string serviceID = "";
            string serviceURLstr = "";
            SqlCommand loginCommand = new SqlCommand($"SELECT KeyName, KeyValue " +
                "FROM Settings " +
                "WHERE KeyName IN ('" +
                    store + ".Username','" +
                    store + ".Password','" +
                    store + ".ServiceId','" +
                    store + ".ServiceUrl')", azureSqlCon);
            SqlDataReader loginReader = loginCommand.ExecuteReader();

            if (loginReader.HasRows)
            {
                while (loginReader.Read())
                {
                    keyName = loginReader.GetString(0);
                    if (keyName == store + ".Username")
                    {
                        username = loginReader.GetString(1);
                    }
                    else if (keyName == store + ".Password")
                    {
                        password = loginReader.GetString(1);
                    }
                    else if (keyName == store + ".ServiceId")
                    {
                        serviceID = loginReader.GetString(1);
                    }
                    else if (keyName == store + ".ServiceUrl")
                    {
                        serviceURLstr = loginReader.GetString(1);
                    }
                }
            }
            else
            {
                serviceURL = null;
                return "";
            }
            loginReader.Close();
            serviceURL = new EndpointAddress(serviceURLstr);
            //CV3DataRequest request = new CV3DataRequest($"<authenticate><user>{username}</user><pass>{password}</pass><serviceID>{serviceID}</serviceID></authenticate>");

            string authentication = $"<authenticate><user>{username}</user><pass>{password}</pass><serviceID>{serviceID}</serviceID></authenticate>";
            return authentication;
        }

        /// <summary>
        /// Add attributes from SearchSpringAttributes to the given DataTable in the JSON format defined by SearchSpring.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="integrationSqlCon"></param>
        /// <returns></returns>
        public static DataTable addAttributes(DataTable table, SqlConnection integrationSqlCon)
        {
            //If/when we move to SQL 2017+, this function can be elimitnated and done in the view with STRING_AGG
            SqlCommand attributeCommand = new SqlCommand("SELECT parentSKU, attribute, retailPrice FROM SearchSpringAttribute", integrationSqlCon);
            SqlDataReader attributeReader = attributeCommand.ExecuteReader();
            table.Columns["attribute"].ReadOnly = false;
            table.Columns["attribute"].MaxLength = 50000;   //Arbitary big number
            if (attributeReader.HasRows)
            {
                string SKU, attribute, retailPrice, currentAttribute;
                EnumerableRowCollection<DataRow> tableRowSearch;
                Dictionary<string, string> attributeDictionary = new Dictionary<string, string>();
                while (attributeReader.Read())
                {
                    SKU = attributeReader.GetString(0);
                    attribute = attributeReader.GetString(1);
                    retailPrice = attributeReader.GetDecimal(2).ToString("0.00");

                    if (Regex.IsMatch(SKU, "([0-9]{2}-[0-9]{3}-[0-9]{2})") || SKU.ToUpper().Contains("RENT"))
                    {
                        if (attributeDictionary.ContainsKey(SKU))
                        {
                            attributeDictionary[SKU] += $",{{'option': '{attribute}', 'price': '{retailPrice}'}}";
                        }
                        else
                        {
                            attributeDictionary.Add(SKU, $"{{'option': '{attribute}', 'price': '{retailPrice}'}}");
                        }
                    }
                }

                foreach(KeyValuePair<string, string> item in attributeDictionary)
                {
                    tableRowSearch = table.AsEnumerable().Where(row => row.Field<string>("sku") == item.Key);
                    if (tableRowSearch != null && tableRowSearch.Count() > 0)
                    {
                        tableRowSearch.First().SetField("attribute", $"[{{'name' : 'Rental Rates', 'values' : [{item.Value}]}}]");
                    }
                }
            }
            attributeReader.Close();
            return table;
        }
    }
}

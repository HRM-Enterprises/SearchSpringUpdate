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
using System.Net.Http;
using System.Configuration;

using CV3ServiceProxy.Services;

using static SearchSpringUpdate.CV3functions;
using static SearchSpringUpdate.DatabaseFunctions;

using log4net;

namespace SearchSpringUpdate
{
    class Program
    {
        static string azureConnectionStr = ConfigurationManager.ConnectionStrings["azureDBConnectionStr"].ConnectionString;
        static string integrationConnectionStr = ConfigurationManager.ConnectionStrings["integrationDBConnectionStr"].ConnectionString;
        static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            SqlConnection azureSqlCon = null;
            SqlConnection integrationSqlCon = null;

            try
            {
                //Uncomment to generate the file without updating DB
                //integrationSqlCon = new SqlConnection(integrationConnectionStr);
                //integrationSqlCon.Open();
                //MemoryStream stream = createCSVFileStream(integrationSqlCon);
                ////writeMemStreamToFile(stream, "output.csv");
                //stream = zipMemoryStream(stream);
                //writeMemStreamToFile(stream, "output.zip");
                //stream.Flush();
                //return;
                log.Debug("*******Begin*******");

                int store = 1;
                azureSqlCon = new SqlConnection(azureConnectionStr);
                azureSqlCon.Open();
                List<Category> categories;
                List<Product> productList = getCV3Products("CV3_" + store, out categories, azureSqlCon);
                integrationSqlCon = new SqlConnection(integrationConnectionStr);
                integrationSqlCon.Open();
                productList = insertCV3Products(productList, store, integrationSqlCon);
                insertCV3Attributes(productList, store, integrationSqlCon);
                insertCV3ImageSet(productList, store, integrationSqlCon);
                insertCV3Filters(productList, store, integrationSqlCon);
                insertCV3Categories(categories, store, integrationSqlCon);
                MemoryStream stream = createCSVFileStream(integrationSqlCon);
                stream = zipMemoryStream(stream, "SearchSpring.csv");
                writeMemStreamToFile(stream, "test.zip");

                log.Debug("*******End*******");
            }
            catch(Exception ex)
            {
                log.Error("Error in Main", ex);
                return;
            }
            finally
            {
                if (azureSqlCon != null)
                {
                    azureSqlCon.Close();
                }
                if (integrationSqlCon != null)
                {
                    integrationSqlCon.Close();
                }
            }
        }
    }
}

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

using static SearchSpringUpdate.CV3functions;
using static SearchSpringUpdate.DatabaseFunctions;

namespace SearchSpringUpdate
{
    public static class SearchSpring
    {
        static string azureConnectionStr = ConfigurationManager.AppSettings["AzureDBConnectionStr"];
        static string integrationConnectionStr = ConfigurationManager.AppSettings["IntegrationDBConnectionStr"];
        static string serviceAddress = ConfigurationManager.AppSettings["ServiceAddress"];
        static string siteID = ConfigurationManager.AppSettings["SearchSpringSiteID"];
        static string secretkey = ConfigurationManager.AppSettings["SearchSpringSecretKey"];

        [FunctionName("SearchSpringUpdate")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            SqlConnection azureSqlCon = null;
            SqlConnection integrationSqlCon = null;
            try
            {
                int store = 1;
                //azureSqlCon = new SqlConnection(azureConnectionStr);
                //azureSqlCon.Open();
                //List<Category> categories;
                //List<Product> productList = getCV3Products("CV3_" + store, "Products", out categories, azureSqlCon);
                integrationSqlCon = new SqlConnection(integrationConnectionStr);
                integrationSqlCon.Open();
                //productList = insertCV3Products(productList, store, integrationSqlCon);
                //insertCV3Attributes(productList, store, integrationSqlCon);
                //insertCV3ImageSet(productList, store, integrationSqlCon);
                //insertCV3Filter(productList, store, integrationSqlCon);
                //insertCV3Categories(categories, store, integrationSqlCon);
                MemoryStream stream = createCSVFileStream(integrationSqlCon);
                stream.Position = 0;
                stream = zipMemoryStream(stream, "SearchSpring.csv");
                stream.Position = 0;
                HttpClientHandler handler = new HttpClientHandler();
                handler.Credentials = new NetworkCredential("iokhmf", "Nm75bMaPDXxJANb29FQmBW96YmufZQon");
                HttpClient client = new HttpClient(handler);
                /*WebClient client = new WebClient();
                client.Credentials = new NetworkCredential(siteID, secretkey);
                Stream postStream = client.OpenWrite(serviceAddress, "POST");
                stream.CopyTo(postStream);
                postStream.Flush();
                postStream.Close();*/
                StreamContent streamContent = new StreamContent(stream);
                MultipartFormDataContent formContent = new MultipartFormDataContent();
                formContent.Add(streamContent, "feedFile");
                HttpResponseMessage httpMessage = await client.PostAsync(serviceAddress, formContent);
                return req.CreateResponse(httpMessage.StatusCode, httpMessage);
            }
            catch(System.Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new { ex.Message }, "application/json");
            }
        }
    }
}

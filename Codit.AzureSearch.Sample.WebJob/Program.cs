using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;
using Codit.AzureSearch.Sample.WebJob.Model;
using Microsoft.Azure.Jobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Formatting = Newtonsoft.Json.Formatting;

namespace Codit.AzureSearch.Sample.WebJob
{
    public class Program
    {
        private static void Main()
        {
            var host = new JobHost();
            host.RunAndBlock();
        }

        public static void ProcessAzureSearchBlob([BlobTrigger("azure-search-files/{name}")] TextReader inputReader)
        {
            var message = new IndexBlobMessage();

            var deserializer = new XmlSerializer(typeof(Product), new XmlRootAttribute("Product"));
            var product = (Product)deserializer.Deserialize(inputReader);
            inputReader.Close();
            
            var jsonString = JsonConvert.SerializeObject(product,Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            message.Content = JObject.Parse(jsonString);

            PostToIndex(message, product.ProductId);
        }

        private static void PostToIndex(IndexBlobMessage message, string productId)
        {
            var requestUri = new Uri(string.Format("{0}/indexes/{1}/docs/index?api-version={2}", message.Url, message.Name, message.ApiVersion));

            // Construct JSON format for indexing 
            var indexObject = new JObject();
            var indexObjectArray = new JArray();
            var itemChild = new JObject { { "@search.action", "upload" } };
            itemChild.Merge(message.Content);
            indexObjectArray.Add(itemChild);
            indexObject.Add("value", indexObjectArray);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", message.AdminApiKey);
                HttpResponseMessage result =
                    client.PostAsync(requestUri, new StringContent(indexObject.ToString()
                        , Encoding.UTF8, "application/json"))
                          .Result;

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Uploaded product {0} to index '{1}'", productId, message.Name);
                }
                else
                {
                    var err = string.Format("Document failed to upload: {0} \r\n",
                                            result.Content.ReadAsStringAsync().Result);
                    throw new Exception(err);
                }
            }
        }
    }
}

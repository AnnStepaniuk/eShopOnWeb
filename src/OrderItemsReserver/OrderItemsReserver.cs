using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrderItemsReserver
{
    public class OrderItemsReserver
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("OrderItemsReserver")]
        public static async Task Run(
            [ServiceBusTrigger("orderitemsqueue", Connection = "SERVICE_BUS_CONNECTION_STRING")] Message orderItemsQueue,
            //[Blob("reserved-order-items/{DateTime}.json", FileAccess.Write)] Stream reservedOrderItem,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING"));
                var blobClient = storageAccount.CreateCloudBlobClient();
                blobClient.DefaultRequestOptions = new BlobRequestOptions
                {
                    RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(5), 3),
                };
                var container = blobClient.GetContainerReference("reserved-order-items");
                var blockBlob = container.GetBlockBlobReference($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}.json");
                await blockBlob.UploadFromStreamAsync(new MemoryStream(orderItemsQueue.Body));
            }
            catch
            {
                await httpClient.PostAsync(
                    Environment.GetEnvironmentVariable("LOGIC_APP_URI"), 
                    new StringContent(Encoding.UTF8.GetString(orderItemsQueue.Body), Encoding.UTF8, "application/json"));
            }
        }
    }
}

using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            [ServiceBusTrigger("%QUEUE_NAME%", Connection = "SERVICE_BUS_CONNECTION_STRING")] Message orderItemsQueueMessage,
            //[Blob("reserved-order-items/{DateTime}.json", FileAccess.Write)] Stream reservedOrderItem,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                await SendMessageToBlob(orderItemsQueueMessage);
            }
            catch
            {
                await InvokeLogicApp(orderItemsQueueMessage);
            }
        }

        private static async Task SendMessageToBlob(Message orderItemsQueueMessage)
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            blobClient.DefaultRequestOptions = new BlobRequestOptions
            {
                RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(5), 3)
            };
            var container = blobClient.GetContainerReference(Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME"));
            var blockBlob = container.GetBlockBlobReference($"Order#{orderItemsQueueMessage.MessageId} - {DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}.json");
            await blockBlob.UploadFromStreamAsync(new MemoryStream(orderItemsQueueMessage.Body));
        }

        private static async Task InvokeLogicApp(Message orderItemsQueueMessage)
        {
            var body = new { OrderId = orderItemsQueueMessage.MessageId, Details = Encoding.UTF8.GetString(orderItemsQueueMessage.Body) };
            await httpClient.PostAsync(
                Environment.GetEnvironmentVariable("LOGIC_APP_URI"),
                new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
        }
    }
}

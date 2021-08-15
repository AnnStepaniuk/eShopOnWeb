using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DeliveryOrderProcessor
{
    public class DeliveryOrderProcessor
    {
        [FunctionName("DeliveryOrderProcessor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var request = await new StreamReader(req.Body).ReadToEndAsync();
            await SendOrderDataToCosmosDb(request);

            return new OkObjectResult("Success");
        }

        private static async Task SendOrderDataToCosmosDb(string request)
        {
            using var client = new CosmosClient(Environment.GetEnvironmentVariable("COSMOSDB_ACCOUNT_ENDPOINT"), Environment.GetEnvironmentVariable("COSMOSDB_PRIMERYKEY"));

            var container = await CreateDatabaseAndContainerIfNotExists(client);

            await container.CreateItemAsync(JsonConvert.DeserializeObject(request));
        }

        private static async Task<Container> CreateDatabaseAndContainerIfNotExists(CosmosClient client)
        {
            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync("EshopDB");
            var targetDatabase = databaseResponse.Database;
            var indexingPolicy = new IndexingPolicy
            {
                Automatic = true,
                IncludedPaths =
                {
                    new IncludedPath
                    {
                        Path = "/*"
                    }
                }
            };

            return (await targetDatabase.CreateContainerIfNotExistsAsync(
                new ContainerProperties("Orders", "/ShippingAddress/ZipCode")
                {
                    IndexingPolicy = indexingPolicy
                },
                10000))
                .Container;
        }
    }
}

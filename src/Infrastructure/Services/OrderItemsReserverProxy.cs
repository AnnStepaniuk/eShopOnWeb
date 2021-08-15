using Microsoft.Azure.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Services
{
    public class OrderItemsReserverProxy : IOrderItemsReserverProxy
    {
        private static IQueueClient queueClient;
        private readonly IConfiguration _configuration;

        public OrderItemsReserverProxy(IConfiguration configuration) => _configuration = configuration;

        public async Task SendAsync<T>(int orderId, T request)
        {
            queueClient = new QueueClient(_configuration["SERVICE_BUS_CONNECTION_STRING"], _configuration["QUEUE_NAME"]);

            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request))) { MessageId = orderId.ToString()};

            await queueClient.SendAsync(message);
            await queueClient.CloseAsync();
        }
    }
}

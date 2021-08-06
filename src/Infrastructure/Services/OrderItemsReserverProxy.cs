using Microsoft.Azure.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Services
{
    public class OrderItemsReserverProxy : IOrderItemsReserverProxy
    {
        const string ServiceBusConnectionString = "Endpoint=sb://eshop-web.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Izz/ETkmaNwN7MYIkFLTUfxZpKovj3j5al2ro3OyEfE=";
        const string QueueName = "orderitemsqueue";
        static IQueueClient queueClient;

        public OrderItemsReserverProxy()
        {
        }

        public async Task SendAsync<T>(T request)
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request)));
            
            await queueClient.SendAsync(message);
            await queueClient.CloseAsync();
        }
    }
}

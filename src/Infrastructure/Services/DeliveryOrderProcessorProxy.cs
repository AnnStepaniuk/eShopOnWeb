using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Services
{
    public class DeliveryOrderProcessorProxy : IDeliveryOrderProcessorProxy
    {
        private readonly HttpClient _httpClient;
        private readonly Lazy<string> _orderItemsReserverUrl;

        public DeliveryOrderProcessorProxy(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _orderItemsReserverUrl = new Lazy<string>(() => configuration["DELIVERY_ORDER_PROCESSOR_URL"]);
        }

        public async Task SendAsync<T>(T request)
        {
            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_orderItemsReserverUrl.Value),
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };

            var result = await _httpClient.SendAsync(message);

            result.EnsureSuccessStatusCode();
        }
    }
}

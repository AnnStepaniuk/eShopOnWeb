using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IDeliveryOrderProcessorProxy
    {
        Task SendAsync<T>(T request);
    }
}
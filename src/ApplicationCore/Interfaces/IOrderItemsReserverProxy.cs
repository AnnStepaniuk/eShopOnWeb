using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IOrderItemsReserverProxy
    {
        Task SendAsync<T>(int orderId, T request);
    }
}
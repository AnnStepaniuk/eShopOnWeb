namespace Microsoft.eShopWeb.ApplicationCore.Entities
{
    public class OrderItemForReserve
    {
        public int ItemId { get; set; }
        public int Quantity { get; }

        public OrderItemForReserve(int itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }
}

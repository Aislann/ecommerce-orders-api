namespace EcommerceOrders.Application.DTOs.Orders
{
    public class UpdateOrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

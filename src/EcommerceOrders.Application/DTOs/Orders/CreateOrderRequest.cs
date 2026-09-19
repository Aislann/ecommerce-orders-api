namespace EcommerceOrders.Application.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}

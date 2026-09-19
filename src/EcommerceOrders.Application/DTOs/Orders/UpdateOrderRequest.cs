namespace EcommerceOrders.Application.DTOs.Orders
{
    public class UpdateOrderRequest
    {
        public int UserId { get; set; }
        public List<UpdateOrderItemRequest> Items { get; set; } = new();
    }
}

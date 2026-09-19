namespace EcommerceOrders.Application.DTOs.Orders
{
    public class OrderItemResponse
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

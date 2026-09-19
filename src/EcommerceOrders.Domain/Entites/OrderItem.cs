using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Domain.Entites
{
    public class OrderItem
    {
        public int Id { get; private set; }
        public int OrderId { get; private set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        public OrderItem(int productId, decimal price, int quantity)
        {
            if (productId <= 0)
                throw new DomainException("ProductId must be greater than zero.");

            if (price <= 0)
                throw new DomainException("Price must be greater than zero."); 

            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }
    }
}

using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Domain.Entites
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal Price { get; private set; }

        private Product() { }

        public Product(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required.");

            if (price <= 0)
                throw new DomainException("Price must be greater than zero.");

            Name = name;
            Price = price;
        }

        internal Product(int id, string name, decimal price) : this(name, price)
        {
            if (id <= 0)
                throw new DomainException("ProductId must be greater than zero.");

            Id = id;
        }
    }
}

namespace EcommerceOrders.Domain.Entites
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
    }
}

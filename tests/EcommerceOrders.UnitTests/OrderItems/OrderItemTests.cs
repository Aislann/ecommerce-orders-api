using EcommerceOrders.Domain.Entities;
using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.UnitTests.OrderItems
{
    public class OrderItemTests
    {
        [Fact]
        public void Should_not_create_order_item_with_invalid_price()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new OrderItem(
                    productId: 1,
                    price: 0m,
                    quantity: 1));

            Assert.Equal("Price must be greater than zero.", exception.Message);
        }

        [Fact]
        public void Should_not_create_order_item_with_invalid_quantity()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 0));

            Assert.Equal("Quantity must be greater than zero.", exception.Message);
        }

        [Fact]
        public void Should_not_create_order_item_without_product()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new OrderItem(
                    productId: 0,
                    price: 100m,
                    quantity: 1));

            Assert.Equal("ProductId must be greater than zero.", exception.Message);
        }
    }
}

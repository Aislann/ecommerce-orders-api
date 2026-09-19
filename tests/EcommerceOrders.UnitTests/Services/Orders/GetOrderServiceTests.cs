using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Application.Services.Orders;
using EcommerceOrders.Domain.Entites;
using EcommerceOrders.Domain.Enums;
using EcommerceOrders.Domain.Exceptions;
using Moq;

namespace EcommerceOrders.UnitTests.Services.Orders
{
    public class GetOrderServiceTests
    {
        [Fact]
        public async Task Should_return_order_when_it_exists()
        {
            var orderRepository = new Mock<IOrderRepository>();

            var product = new Product(
                1,
                "Product 1",
                100m);

            var item = new OrderItem(
                product.Id,
                product.Price,
                2);

            var order = new Order(
                1,
                [item]);

            orderRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var service = new GetOrderService(
                orderRepository.Object);

            var result = await service.ExecuteAsync(1);

            Assert.Equal(order.Id, result.Id);
            Assert.Equal(order.UserId, result.UserId);
            Assert.Equal(OrderStatus.Started, result.Status);
            Assert.Single(result.Items);
            Assert.Equal(product.Id, result.Items[0].ProductId);
            Assert.Equal(product.Price, result.Items[0].Price);
            Assert.Equal(2, result.Items[0].Quantity);
        }

        [Fact]
        public async Task Should_throw_when_order_does_not_exist()
        {
            var orderRepository = new Mock<IOrderRepository>();

            orderRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var service = new GetOrderService(
                orderRepository.Object);

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => service.ExecuteAsync(1));

            Assert.Equal("Order not found.", exception.Message);
        }
    }
}

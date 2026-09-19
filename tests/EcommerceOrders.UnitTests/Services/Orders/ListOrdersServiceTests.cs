using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Application.Services.Orders;
using EcommerceOrders.Domain.Entities;
using EcommerceOrders.Domain.Enums;
using Moq;

namespace EcommerceOrders.UnitTests.Services.Orders
{
    public class ListOrdersServiceTests
    {
        [Fact]
        public async Task Should_return_all_orders()
        {
            var orderRepository = new Mock<IOrderRepository>();

            var firstOrder = new Order(
                1,
                [
                    new OrderItem(
                    1,
                    100m,
                    2)
                ]);

            var secondOrder = new Order(
                2,
                [
                    new OrderItem(
                    2,
                    50m,
                    1)
                ]);

            IReadOnlyList<Order> orders =
            [
                firstOrder,
            secondOrder
            ];

            orderRepository
                .Setup(repository => repository.GetAllAsync(
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            var service = new ListOrdersService(
                orderRepository.Object);

            var result = await service.ExecuteAsync();

            Assert.Equal(2, result.Count);

            Assert.Equal(firstOrder.UserId, result[0].UserId);
            Assert.Equal(OrderStatus.Started, result[0].Status);
            Assert.Single(result[0].Items);
            Assert.Equal(1, result[0].Items[0].ProductId);
            Assert.Equal(100m, result[0].Items[0].Price);
            Assert.Equal(2, result[0].Items[0].Quantity);

            Assert.Equal(secondOrder.UserId, result[1].UserId);
            Assert.Single(result[1].Items);

            orderRepository.Verify(
                repository => repository.GetAllAsync(
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Should_return_orders_filtered_by_status()
        {
            var orderRepository = new Mock<IOrderRepository>();

            var order = new Order(
                1,
                [
                    new OrderItem(
                    1,
                    100m,
                    1)
                ]);

            order.Process();

            IReadOnlyList<Order> orders =
            [
                order
            ];

            orderRepository
                .Setup(repository => repository.GetAllAsync(
                    OrderStatus.Processed,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            var service = new ListOrdersService(
                orderRepository.Object);

            var result = await service.ExecuteAsync(
                OrderStatus.Processed);

            Assert.Single(result);
            Assert.Equal(OrderStatus.Processed, result[0].Status);

            orderRepository.Verify(
                repository => repository.GetAllAsync(
                    OrderStatus.Processed,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

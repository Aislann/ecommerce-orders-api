using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Application.Services.Orders;
using EcommerceOrders.Domain.Entities;
using EcommerceOrders.Domain.Exceptions;
using Moq;

namespace EcommerceOrders.UnitTests.Services.Orders
{
    public class DeleteOrderServiceTests
    {
        [Fact]
        public async Task Should_delete_order()
        {
            var orderRepository = new Mock<IOrderRepository>();

            var order = new Order(
                1,
                1,
                [
                    new OrderItem(
                    1,
                    100m,
                    1)
                ]);

            orderRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepository
                .Setup(repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new DeleteOrderService(
                orderRepository.Object);

            await service.ExecuteAsync(1);

            orderRepository.Verify(
                repository => repository.Delete(order),
                Times.Once);

            orderRepository.Verify(
                repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
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

            var service = new DeleteOrderService(
                orderRepository.Object);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => service.ExecuteAsync(1));

            Assert.Equal(
                "Order not found.",
                exception.Message);

            orderRepository.Verify(
                repository => repository.Delete(
                    It.IsAny<Order>()),
                Times.Never);

            orderRepository.Verify(
                repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}

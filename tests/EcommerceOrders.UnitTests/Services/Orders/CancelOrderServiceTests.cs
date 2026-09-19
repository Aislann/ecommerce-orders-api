using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Application.Services.Orders;
using EcommerceOrders.Domain.Entites;
using EcommerceOrders.Domain.Enums;
using EcommerceOrders.Domain.Exceptions;
using Moq;

namespace EcommerceOrders.UnitTests.Services.Orders
{
    public class CancelOrderServiceTests
    {
        [Fact]
        public async Task Should_cancel_started_order()
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

            var service = new CancelOrderService(
                orderRepository.Object);

            var result = await service.ExecuteAsync(1);

            Assert.Equal(1, result.Id);
            Assert.Equal(OrderStatus.Canceled, result.Status);

            orderRepository.Verify(
                repository => repository.Update(order),
                Times.Once);

            orderRepository.Verify(
                repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Should_cancel_processed_order()
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

            order.Process();

            orderRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepository
                .Setup(repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new CancelOrderService(
                orderRepository.Object);

            var result = await service.ExecuteAsync(1);

            Assert.Equal(OrderStatus.Canceled, result.Status);

            orderRepository.Verify(
                repository => repository.Update(order),
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

            var service = new CancelOrderService(
                orderRepository.Object);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => service.ExecuteAsync(1));

            Assert.Equal("Order not found.", exception.Message);

            orderRepository.Verify(
                repository => repository.Update(
                    It.IsAny<Order>()),
                Times.Never);

            orderRepository.Verify(
                repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_not_cancel_shipped_order()
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

            order.Process();
            order.Ship();

            orderRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var service = new CancelOrderService(
                orderRepository.Object);

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                () => service.ExecuteAsync(1));

            Assert.Equal(
                "Only initiated or processed orders can be canceled.",
                exception.Message);

            orderRepository.Verify(
                repository => repository.Update(
                    It.IsAny<Order>()),
                Times.Never);

            orderRepository.Verify(
                repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_not_cancel_already_canceled_order()
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

            order.Cancel();

            orderRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var service = new CancelOrderService(
                orderRepository.Object);

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                () => service.ExecuteAsync(1));

            Assert.Equal(
                "Only initiated or processed orders can be canceled.",
                exception.Message);

            orderRepository.Verify(
                repository => repository.Update(
                    It.IsAny<Order>()),
                Times.Never);

            orderRepository.Verify(
                repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}

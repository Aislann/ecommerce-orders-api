using EcommerceOrders.Application.DTOs.Orders;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Application.Services.Orders;
using EcommerceOrders.Domain.Entites;
using EcommerceOrders.Domain.Exceptions;
using Moq;

namespace EcommerceOrders.UnitTests.Services.Orders
{
    public class UpdateOrderServiceTests
    {
        [Fact]
        public async Task Should_update_order()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var userRepository = new Mock<IUserRepository>();
            var productRepository = new Mock<IProductRepository>();

            var user = new User(
                1,
                "Aislan Oliveira",
                "aislan@example.com");

            var product = new Product(
                1,
                "Product 1",
                100m);

            var order = new Order(
                1,
                1,
                [
                    new OrderItem(
                        1,
                        50m,
                        1)
                ]);

            orderRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            userRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            productRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            orderRepository
                .Setup(repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new UpdateOrderService(
                orderRepository.Object,
                userRepository.Object,
                productRepository.Object);

            var request = new UpdateOrderRequest
            {
                UserId = 1,
                Items =
                [
                    new UpdateOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 3
                }
                ]
            };

            var result = await service.ExecuteAsync(
                1,
                request);

            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.UserId);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Items[0].ProductId);
            Assert.Equal(100m, result.Items[0].Price);
            Assert.Equal(3, result.Items[0].Quantity);

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
            var userRepository = new Mock<IUserRepository>();
            var productRepository = new Mock<IProductRepository>();

            orderRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var service = new UpdateOrderService(
                orderRepository.Object,
                userRepository.Object,
                productRepository.Object);

            var request = new UpdateOrderRequest
            {
                UserId = 1,
                Items =
                [
                    new UpdateOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 1
                }
                ]
            };

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => service.ExecuteAsync(1, request));

            Assert.Equal("Order not found.", exception.Message);

            userRepository.Verify(
                repository => repository.GetByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_throw_when_buyer_does_not_exist()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var userRepository = new Mock<IUserRepository>();
            var productRepository = new Mock<IProductRepository>();

            var order = new Order(
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

            userRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var service = new UpdateOrderService(
                orderRepository.Object,
                userRepository.Object,
                productRepository.Object);

            var request = new UpdateOrderRequest
            {
                UserId = 1,
                Items =
                [
                    new UpdateOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 1
                }
                ]
            };

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => service.ExecuteAsync(1, request));

            Assert.Equal("Buyer not found.", exception.Message);

            productRepository.Verify(
                repository => repository.GetByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            orderRepository.Verify(
                repository => repository.Update(
                    It.IsAny<Order>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_not_update_processed_order()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var userRepository = new Mock<IUserRepository>();
            var productRepository = new Mock<IProductRepository>();

            var user = new User(
                1,
                "Aislan Oliveira",
                "aislan@example.com");

            var product = new Product(
                1,
                "Product 1",
                100m);

            var order = new Order(
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

            userRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            productRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var service = new UpdateOrderService(
                orderRepository.Object,
                userRepository.Object,
                productRepository.Object);

            var request = new UpdateOrderRequest
            {
                UserId = 1,
                Items =
                [
                    new UpdateOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
                ]
            };

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                () => service.ExecuteAsync(1, request));

            Assert.Equal(
                "Only initiated orders can be changed.",
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

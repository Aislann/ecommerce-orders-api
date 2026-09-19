using EcommerceOrders.Application.DTOs.Orders;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Application.Services.Orders;
using EcommerceOrders.Domain.Entites;
using EcommerceOrders.Domain.Enums;
using EcommerceOrders.Domain.Exceptions;
using Moq;

namespace EcommerceOrders.UnitTests.Services.Orders
{
    public class CreateOrderServiceTests
    {
        [Fact]
        public async Task Should_create_order()
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
                .Setup(repository => repository.AddAsync(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            orderRepository
                .Setup(repository => repository.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new CreateOrderService(
                orderRepository.Object,
                userRepository.Object,
                productRepository.Object);

            var request = new CreateOrderRequest
            {
                UserId = 1,
                Items =
                [
                    new CreateOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
                ]
            };

            var result = await service.ExecuteAsync(request);

            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(OrderStatus.Started, result.Status);
            Assert.Single(result.Items);
            Assert.Equal(product.Id, result.Items[0].ProductId);
            Assert.Equal(product.Price, result.Items[0].Price);
            Assert.Equal(2, result.Items[0].Quantity);
        }

        [Fact]
        public async Task Should_not_create_order_when_buyer_does_not_exist()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var userRepository = new Mock<IUserRepository>();
            var productRepository = new Mock<IProductRepository>();

            userRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var service = new CreateOrderService(
                orderRepository.Object,
                userRepository.Object,
                productRepository.Object);

            var request = new CreateOrderRequest
            {
                UserId = 1,
                Items =
                [
                    new CreateOrderItemRequest
            {
                ProductId = 1,
                Quantity = 2
            }
                ]
            };

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => service.ExecuteAsync(request));

            Assert.Equal("Buyer not found.", exception.Message);

            productRepository.Verify(
                repository => repository.GetByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            orderRepository.Verify(
                repository => repository.AddAsync(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_not_create_order_when_product_does_not_exist()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var userRepository = new Mock<IUserRepository>();
            var productRepository = new Mock<IProductRepository>();

            var user = new User(
                1,
                "John Doe",
                "john@example.com");

            userRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            productRepository
                .Setup(repository => repository.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var service = new CreateOrderService(
                orderRepository.Object,
                userRepository.Object,
                productRepository.Object);

            var request = new CreateOrderRequest
            {
                UserId = 1,
                Items =
                [
                    new CreateOrderItemRequest
            {
                ProductId = 1,
                Quantity = 2
            }
                ]
            };

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => service.ExecuteAsync(request));

            Assert.Equal("Product 1 not found.", exception.Message);

            orderRepository.Verify(
                repository => repository.AddAsync(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}

using EcommerceOrders.Domain.Entites;
using EcommerceOrders.Domain.Enums;
using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.UnitTests.Orders
{
    public class OrderTests
    {
        [Fact]
        public void Should_create_order_with_started_status()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 2)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            Assert.Equal(OrderStatus.Started, order.Status);
            Assert.Single(order.OrderItems);
        }

        [Fact]
        public void Should_not_update_processed_order()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 2)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            order.Process();

            var newItems = new[]
            {
                new OrderItem(
                    productId: 2,
                    price: 200m,
                    quantity: 1)
            };

            var exception = Assert.Throws<DomainException>(() =>
                order.Update(
                    userId: 2,
                    orderItems: newItems));

            Assert.Equal("Only initiated orders can be changed.", exception.Message);
        }

        [Fact]
        public void Should_update_started_order()
        {
            var initialItems = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 2)
            };

            var order = new Order(
                userId: 1,
                orderItems: initialItems);

            var newItems = new[]
            {
                new OrderItem(
                    productId: 2,
                    price: 200m,
                    quantity: 1)
            };

            order.Update(
                userId: 2,
                orderItems: newItems);

            Assert.Equal(2, order.UserId);
            Assert.Single(order.OrderItems);
            Assert.Equal(2, order.OrderItems.First().ProductId);
            Assert.Equal(200m, order.OrderItems.First().Price);
            Assert.NotNull(order.UpdatedAt);
        }

        [Fact]
        public void Should_cancel_started_order()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 1)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            order.Cancel();

            Assert.Equal(OrderStatus.Canceled, order.Status);
            Assert.NotNull(order.UpdatedAt);
        }

        [Fact]
        public void Should_cancel_processed_order()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 1)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            order.Process();
            order.Cancel();

            Assert.Equal(OrderStatus.Canceled, order.Status);
            Assert.NotNull(order.UpdatedAt);
        }

        [Fact]
        public void Should_not_cancel_shipped_order()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 1)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            order.Process();
            order.Ship();

            var exception = Assert.Throws<DomainException>(() =>
                order.Cancel());

            Assert.Equal("Only initiated or processed orders can be canceled.", exception.Message);
        }

        [Fact]
        public void Should_not_cancel_canceled_order()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 1)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            order.Cancel();

            var exception = Assert.Throws<DomainException>(() =>
                order.Cancel());

            Assert.Equal("Only initiated or processed orders can be canceled.", exception.Message);
        }

        [Fact]
        public void Should_process_started_order()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 1)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            order.Process();

            Assert.Equal(OrderStatus.Processed, order.Status);
            Assert.NotNull(order.UpdatedAt);
        }

        [Fact]
        public void Should_not_process_processed_order()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 1)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            order.Process();

            var exception = Assert.Throws<DomainException>(() =>
                order.Process());

            Assert.Equal("Only initiated orders can be processed.", exception.Message);
        }

        [Fact]
        public void Should_ship_processed_order()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 1)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            order.Process();
            order.Ship();

            Assert.Equal(OrderStatus.Shipped, order.Status);
            Assert.NotNull(order.UpdatedAt);
        }

        [Fact]
        public void Should_not_ship_started_order()
        {
            var items = new[]
            {
                new OrderItem(
                    productId: 1,
                    price: 100m,
                    quantity: 1)
            };

            var order = new Order(
                userId: 1,
                orderItems: items);

            var exception = Assert.Throws<DomainException>(() =>
                order.Ship());

            Assert.Equal("Only processed orders can be sent.", exception.Message);
        }
    }
}

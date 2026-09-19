using EcommerceOrders.Application.DTOs.Orders;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Domain.Entites;
using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Application.Services.Orders
{
    public class UpdateOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        public UpdateOrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        public async Task<OrderResponse> ExecuteAsync(
            int orderId,
            UpdateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(
                orderId,
                cancellationToken);

            if (order is null)
                throw new DomainException("Order not found.");

            var user = await _userRepository.GetByIdAsync(
                request.UserId,
                cancellationToken);

            if (user is null)
                throw new DomainException("Buyer not found.");

            var items = new List<OrderItem>();

            foreach (var itemRequest in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(
                    itemRequest.ProductId,
                    cancellationToken);

                if (product is null)
                    throw new DomainException(
                        $"Product {itemRequest.ProductId} not found.");

                var item = new OrderItem(
                    product.Id,
                    product.Price,
                    itemRequest.Quantity);

                items.Add(item);
            }

            order.Update(
                user.Id,
                items);

            _orderRepository.Update(order);

            await _orderRepository.SaveChangesAsync(
                cancellationToken);

            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.Items
                    .Select(item => new OrderItemResponse
                    {
                        ProductId = item.ProductId,
                        Price = item.Price,
                        Quantity = item.Quantity
                    })
                    .ToList()
            };
        }
    }
}

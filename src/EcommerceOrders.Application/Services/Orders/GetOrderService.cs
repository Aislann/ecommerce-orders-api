using EcommerceOrders.Application.DTOs.Orders;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Application.Services.Orders
{
    public class GetOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderResponse> ExecuteAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(
                orderId,
                cancellationToken);

            if (order is null)
                throw new NotFoundException("Order not found.");

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

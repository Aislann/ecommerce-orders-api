using EcommerceOrders.Application.DTOs.Orders;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Domain.Enums;

namespace EcommerceOrders.Application.Services.Orders
{
    public class ListOrdersService
    {
        private readonly IOrderRepository _orderRepository;

        public ListOrdersService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IReadOnlyList<OrderResponse>> ExecuteAsync(
            OrderStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAllAsync(
                status,
                cancellationToken);

            return orders
                .Select(order => new OrderResponse
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
                })
                .ToList();
        }
    }
}

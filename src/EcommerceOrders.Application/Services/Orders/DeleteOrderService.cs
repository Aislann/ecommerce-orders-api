using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Application.Services.Orders
{
    public class DeleteOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public DeleteOrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task ExecuteAsync(
            int orderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(
                orderId,
                cancellationToken);

            if (order is null)
                throw new DomainException("Order not found.");

            _orderRepository.Delete(order);

            await _orderRepository.SaveChangesAsync(
                cancellationToken);
        }
    }
}

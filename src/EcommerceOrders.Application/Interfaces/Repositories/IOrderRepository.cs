using EcommerceOrders.Domain.Entites;
using EcommerceOrders.Domain.Enums;

namespace EcommerceOrders.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken = default);

        Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetAllAsync(OrderStatus? status = null, CancellationToken cancellationToken = default);

        void Update(Order order);

        void Delete(Order order);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

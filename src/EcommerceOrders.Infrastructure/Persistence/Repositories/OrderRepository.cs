using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Domain.Entites;
using EcommerceOrders.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrders.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            Order order,
            CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(
                order,
                cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(order => order.Items)
                .FirstOrDefaultAsync(
                    order => order.Id == id,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetAllAsync(
            OrderStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Orders
                .Include(order => order.Items)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(
                    order => order.Status == status.Value);
            }

            return await query
                .OrderByDescending(order => order.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

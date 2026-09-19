using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrders.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .FirstOrDefaultAsync(
                    product => product.Id == id,
                    cancellationToken);
        }
    }
}

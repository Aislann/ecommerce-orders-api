using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrders.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    user => user.Id == id,
                    cancellationToken);
        }
    }
}

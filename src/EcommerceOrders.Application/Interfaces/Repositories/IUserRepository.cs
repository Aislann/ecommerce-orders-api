using EcommerceOrders.Domain.Entites;

namespace EcommerceOrders.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}

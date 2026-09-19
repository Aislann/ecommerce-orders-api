using EcommerceOrders.Domain.Entites;

namespace EcommerceOrders.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}

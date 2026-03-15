using SmartCart.Domain.Entities;

namespace SmartCart.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default);
        Task<Product?> GetAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(Product product, CancellationToken ct = default);
    }
}

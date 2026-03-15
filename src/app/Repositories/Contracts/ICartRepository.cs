using SmartCart.Domain.Entities;

namespace SmartCart.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetOrCreateAsync(Guid cartId, CancellationToken ct = default);
        Task<Cart?> GetAsync(Guid cartId, CancellationToken ct = default);
        Task UpdateAsync(Cart cart, CancellationToken ct = default);
    }
}

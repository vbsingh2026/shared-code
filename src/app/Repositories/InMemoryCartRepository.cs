using SmartCart.Application.Interfaces;
using SmartCart.Domain.Entities;
using System.Collections.Concurrent;

namespace SmartCart.Infrastructure.Repositories
{
    public sealed class InMemoryCartRepository : ICartRepository
    {
        private readonly ConcurrentDictionary<Guid, Cart> _carts = new();

        public Task<Cart> GetOrCreateAsync(Guid cartId, CancellationToken ct = default)
        {
            if (cartId == Guid.Empty)
                cartId = Guid.NewGuid();

            var cart = _carts.GetOrAdd(cartId, id => new Cart { Id = id });
            return Task.FromResult(cart);
        }

        public Task<Cart?> GetAsync(Guid cartId, CancellationToken ct = default)
        {
            _carts.TryGetValue(cartId, out var cart);
            return Task.FromResult(cart);
        }

        public Task UpdateAsync(Cart cart, CancellationToken ct = default)
        {
            _carts[cart.Id] = cart;
            return Task.CompletedTask;
        }
    }
}

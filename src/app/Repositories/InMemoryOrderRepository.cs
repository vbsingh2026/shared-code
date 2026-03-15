using SmartCart.Application.Interfaces;
using SmartCart.Domain.Entities;
using System.Collections.Concurrent;

namespace SmartCart.Infrastructure.Repositories
{
    public sealed class InMemoryOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<Guid, Order> _orders = new();

        public Task CreateAsync(Order order, CancellationToken ct = default)
        {
            _orders[order.Id] = order;
            return Task.CompletedTask;
        }

        public Task<Order?> GetAsync(Guid orderId, CancellationToken ct = default)
        {
            _orders.TryGetValue(orderId, out var order);
            return Task.FromResult(order);
        }
    }
}

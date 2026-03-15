using SmartCart.Domain.Entities;

namespace SmartCart.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task CreateAsync(Order order, CancellationToken ct = default);
        Task<Order?> GetAsync(Guid orderId, CancellationToken ct = default);
    }
}

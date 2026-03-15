using Microsoft.Extensions.Options;
using SmartCart.Application.DTOs;
using SmartCart.Domain.Entities;
using System.Text.Json;

namespace SmartCart.Application.Interfaces
{
    public interface ISmartCartService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync(CancellationToken ct = default);
        Task<CartDTO> GetCartAsync(Guid cartId, CancellationToken ct = default);
        Task<Result> AddOrUpdateCartItemAsync(Guid cartId, Guid productId, int quantity, CancellationToken ct = default);
        Task<Result> ApplyCouponAsync(Guid cartId, string couponCode, CancellationToken ct = default);
        Task<(Result result, OrderDTO? order)> CheckoutAsync(Guid cartId, CancellationToken ct = default);
        Task<OrderDTO?> GetOrderAsync(Guid orderId, CancellationToken ct = default);
    }

    public sealed class Result
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public string? Code { get; init; }

        public object? Data { get; init; }

        public static Result Ok(object? data) => new()
        {
            Success = true,
            Data = JsonSerializer.Serialize(data,
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        })
        };

        public static Result Fail(string message, string code) => new() { Success = false, Message = message, Code = code };
    }
}

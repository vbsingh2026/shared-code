namespace SmartCart.Application.DTOs
{
    public sealed class ProductDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public decimal Price { get; init; }
        public int Stock { get; init; }
    }
}

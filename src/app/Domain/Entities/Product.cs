namespace SmartCart.Domain.Entities
{
    public sealed class Product
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}

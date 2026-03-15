namespace SmartCart.Domain.Entities
{
    public sealed class Order
    {
        public Guid Id { get; init; }
        public List<CartItem> Items { get; init; } = new();
        public decimal Subtotal { get; init; }
        public decimal Discount { get; init; }
        public decimal Tax { get; init; }
        public decimal GrandTotal { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}

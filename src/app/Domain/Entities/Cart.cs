namespace SmartCart.Domain.Entities
{
    public sealed class Cart
    {
        public Guid Id { get; init; }
        public List<CartItem> Items { get; } = new();
        public string? CouponCode { get; set; }
    }
}

namespace SmartCart.Application.DTOs
{
    public sealed class CartItemDTO
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = null!;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    public sealed class CartDTO
    {
        public Guid Id { get; init; }
        public List<CartItemDTO> Items { get; init; } = new();
        public string? CouponCode { get; init; }
        public decimal Subtotal { get; init; }
        public decimal Discount { get; init; }
        public decimal Tax { get; init; }
        public decimal GrandTotal { get; init; }
    }
}

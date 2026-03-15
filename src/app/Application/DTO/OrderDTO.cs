namespace SmartCart.Application.DTOs
{
    public sealed class OrderDTO
    {
        public Guid OrderId { get; init; }
        public List<CartItemDTO> Items { get; init; } = new();
        public decimal Subtotal { get; init; }
        public decimal Discount { get; init; }
        public decimal Tax { get; init; }
        public decimal GrandTotal { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}

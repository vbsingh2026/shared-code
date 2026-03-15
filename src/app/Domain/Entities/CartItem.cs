namespace SmartCart.Domain.Entities
{
    public sealed class CartItem
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; init; }
    }
}

namespace SmartCart.Application.Interfaces
{
    public interface ICouponStrategy
    {
        decimal CalculateDiscount(decimal subtotal);
        bool IsApplicable(decimal subtotal);
    }
}

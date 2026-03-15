using SmartCart.Application.Interfaces;

namespace SmartCart.Application.Coupons.Strategies
{
    public sealed class FlatCouponStrategy : ICouponStrategy
    {
        public bool IsApplicable(decimal subtotal) => subtotal >= 500m;

        public decimal CalculateDiscount(decimal subtotal)
        {
            if (!IsApplicable(subtotal)) return 0m;
            return 50m;
        }
    }
}

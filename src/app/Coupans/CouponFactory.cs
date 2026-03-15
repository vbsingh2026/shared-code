using SmartCart.Application.Interfaces;
using SmartCart.Application.Coupons.Strategies;

namespace SmartCart.Application.Coupons
{
    public sealed class CouponFactory : ICouponFactory
    {
        public ICouponStrategy? GetStrategy(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            return code.Trim().ToUpperInvariant() switch
            {
                "FLAT50" => new FlatCouponStrategy(),
                "SAVE10" => new PercentageCouponStrategy(),
                _ => null
            };
        }
    }
}

using SmartCart.Application.Interfaces;

namespace SmartCart.Application.Coupons.Strategies
{
    public sealed class PercentageCouponStrategy : ICouponStrategy
    {
        private const decimal Rate = 0.10m;
        private const decimal Max = 200m;
        private const decimal MinSubtotal = 1000m;

        public bool IsApplicable(decimal subtotal) => subtotal >= MinSubtotal;

        public decimal CalculateDiscount(decimal subtotal)
        {
            if (!IsApplicable(subtotal)) 
                return 0m;

            var discount = subtotal * Rate;
            return discount > Max ? Max : discount;
        }
    }
}

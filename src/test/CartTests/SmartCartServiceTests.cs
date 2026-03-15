using SmartCart.Application.Services;
using SmartCart.Application.Coupons;
using SmartCart.Infrastructure.Repositories;
using Xunit;

namespace SmartCart.Tests
{
    public sealed class SmartCartServiceTests
    {
        private readonly SmartCart.Application.Services.SmartCartService _service;
        private readonly InMemoryProductRepository _productRepo;
        private readonly InMemoryCartRepository _cartRepo;
        private readonly InMemoryOrderRepository _orderRepo;
        private readonly CouponFactory _couponFactory;

        public SmartCartServiceTests()
        {
            _productRepo = new InMemoryProductRepository();
            _cartRepo = new InMemoryCartRepository();
            _orderRepo = new InMemoryOrderRepository();
            _couponFactory = new CouponFactory();
            _service = new SmartCart.Application.Services.SmartCartService(_productRepo, _cartRepo, _orderRepo, _couponFactory);
        }

        [Fact]
        public async Task Save10_Coupon_Not_Applicable_Under_1000()
        {
            var cartId = Guid.NewGuid();
            // Add one Keyboard (1200) -> It is >=1000 so applicable. For negative test use USB Cable 150
            var prod = (await _productRepo.GetAllAsync()).First(p => p.Price == 150m);
            await _service.AddOrUpdateCartItemAsync(cartId, prod.Id, 1);
            var res = await _service.ApplyCouponAsync(cartId, "SAVE10");
            Assert.False(res.Success);
            Assert.Equal("COUPON_NOT_APPLICABLE", res.Code);
        }

        [Fact]
        public async Task Checkout_Calculates_Pricing_Correctly()
        {
            var cartId = Guid.NewGuid();
            var keyboard = (await _productRepo.GetAllAsync()).First(p => p.Name == "Keyboard");
            await _service.AddOrUpdateCartItemAsync(cartId, keyboard.Id, 1); // 1200
            await _service.ApplyCouponAsync(cartId, "SAVE10"); // 10% = 120, max 200 -> 120
            var (result, order) = await _service.CheckoutAsync(cartId);
            Assert.True(result.Success);
            Assert.NotNull(order);
            Assert.Equal(1200m, order!.Subtotal);
            Assert.Equal(120m, order.Discount);
            Assert.Equal(Math.Round((1200 - 120) * 0.18m, 2), order.Tax);
            Assert.Equal(Math.Round((1200 - 120) + Math.Round((1200 - 120) * 0.18m, 2), 2), order.GrandTotal);
        }

        [Fact]
        public async Task Checkout_Fails_When_Stock_Insufficient()
        {
            var cartId = Guid.NewGuid();
            var keyboard = (await _productRepo.GetAllAsync()).First(p => p.Name == "Keyboard");
            // set stock low
            keyboard.Stock = 1;
            await _productRepo.UpdateAsync(keyboard);
            // try to add quantity > stock
            var addRes = await _service.AddOrUpdateCartItemAsync(cartId, keyboard.Id, 5);
            Assert.False(addRes.Success);
            Assert.Equal("STOCK_EXCEEDED", addRes.Code);
        }
    }
}

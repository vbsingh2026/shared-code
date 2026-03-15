using Microsoft.AspNetCore.Mvc;
using SmartCart.Application.Interfaces;
using SmartCart.Application.DTOs;

namespace SmartCart.Api.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public sealed class CartController : ControllerBase
    {
        private readonly ISmartCartService _service;

        public CartController(ISmartCartService service) => _service = service;

        public sealed record AddItemRequest(Guid CartId, Guid ProductId, int Quantity);
        

        [HttpPost("items")]
        public async Task<IActionResult> AddOrUpdate([FromBody] AddItemRequest req)
        {
            Result result = await _service.AddOrUpdateCartItemAsync(req.CartId, req.ProductId, req.Quantity);
            
            if (!result.Success) 
                return BadRequest(new { message = result.Message, code = result.Code });
            
            return Ok(result.Data);
        }

        [HttpGet("{cartId:guid}")]
        public async Task<IActionResult> GetCart(Guid cartId)
        {
            CartDTO dto = await _service.GetCartAsync(cartId);
            return Ok(dto);
        }

        public sealed class ApplyCouponRequest { public string CouponCode { get; init; } = null!; }

        [HttpPost("{cartId:guid}/apply-coupon")]
        public async Task<IActionResult> ApplyCoupon(Guid cartId, [FromBody] ApplyCouponRequest req)
        {
            var result = await _service.ApplyCouponAsync(cartId, req.CouponCode);
            if (!result.Success) 
                return BadRequest(new { message = result.Message, code = result.Code });

            return Ok(result.Data);
        }

        [HttpPost("{cartId:guid}/checkout")]
        public async Task<IActionResult> Checkout(Guid cartId)
        {
            var (result, order) = await _service.CheckoutAsync(cartId);
            if (!result.Success) return BadRequest(new { message = result.Message, code = result.Code });
            return Ok(order);
        }
    }
}

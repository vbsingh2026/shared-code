using Microsoft.AspNetCore.Mvc;
using SmartCart.Application.DTOs;
using SmartCart.Application.Interfaces;

namespace SmartCart.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public sealed class OrdersController : ControllerBase
    {
        private readonly ISmartCartService _service;

        public OrdersController(ISmartCartService service) => _service = service;

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            OrderDTO? order = await _service.GetOrderAsync(orderId);
            if (order == null) return NotFound(new { message = "Order not found", code = "ORDER_NOT_FOUND" });
            return Ok(order);
        }
    }
}

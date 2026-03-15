using Microsoft.AspNetCore.Mvc;
using SmartCart.Application.DTOs;
using SmartCart.Application.Interfaces;

namespace SmartCart.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly ISmartCartService _service;

        public ProductsController(ISmartCartService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<ProductDTO> products = await _service.GetProductsAsync();
            return Ok(products);
        }
    }
}

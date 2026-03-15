using SmartCart.Application.Interfaces;
using SmartCart.Application.DTOs;
using SmartCart.Domain.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SmartCart.Application.Services
{
    public sealed class SmartCartService(IProductRepository _productRepository, ICartRepository _cartRepository,
        IOrderRepository _orderRepository, ICouponFactory _couponFactory) : ISmartCartService
    {
        private static readonly object _checkoutLock = new();

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync(CancellationToken ct = default)
        {
            var products = await _productRepository.GetAllAsync(ct);
            return products.Select(p => new ProductDTO { Id = p.Id, Name = p.Name, Price = p.Price, Stock = p.Stock });
        }

        public async Task<CartDTO> GetCartAsync(Guid cartId, CancellationToken ct = default)
        {
            Cart? cart = await _cartRepository.GetAsync(cartId, ct);

            return await CalculationOnCartAsync(cart, ct);
        }

        public async Task<Result> AddOrUpdateCartItemAsync(Guid cartId, Guid productId, int quantity, CancellationToken ct = default)
        {
            if (quantity <= 0)
                return Result.Fail("Quantity must be > 0", "INVALID_QUANTITY");

            Product? product = await _productRepository.GetAsync(productId, ct);
            if (product == null) 
                return Result.Fail("Product not found", "PRODUCT_NOT_FOUND");

            if (quantity > product.Stock) 
                return Result.Fail("Requested quantity exceeds available stock", "STOCK_EXCEEDED");

            Cart cart = await _cartRepository.GetOrCreateAsync(cartId, ct);

            CartItem? existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existing == null)
            {
                cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity, UnitPrice = product.Price });
            }
            else
            {
                if (quantity > product.Stock) 
                    return Result.Fail("Requested quantity exceeds available stock", "STOCK_EXCEEDED");

                existing.Quantity = quantity;
            }

            await _cartRepository.UpdateAsync(cart, ct);//Update cart in repository

			return Result.Ok(cart);

			//return Result.Ok();

			//var options = new JsonSerializerOptions
			//{
			//	PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			//};

			//         return Result.Ok(JsonSerializer.Serialize(cart, options));
		}

        public async Task<Result> ApplyCouponAsync(Guid cartId, string couponCode, CancellationToken ct = default)
        {
            Cart cart = await _cartRepository.GetOrCreateAsync(cartId, ct);
            CartDTO cartDto = await CalculationOnCartAsync(cart, ct);
            ICouponStrategy? strategy = _couponFactory.GetStrategy(couponCode ?? string.Empty);

            if (strategy == null) 
                return Result.Fail("Invalid coupon code", "INVALID_COUPON");

            if (!strategy.IsApplicable(cartDto.Subtotal)) 
                return Result.Fail("Coupon not applicable for current subtotal", "COUPON_NOT_APPLICABLE");

            cart.CouponCode = couponCode.ToUpperInvariant();
            await _cartRepository.UpdateAsync(cart, ct);

			cartDto = await CalculationOnCartAsync(cart, ct);
			return Result.Ok(cartDto);
		}

        public async Task<(Result result, OrderDTO? order)> CheckoutAsync(Guid cartId, CancellationToken ct = default)
        {
            // Very simple Unit of Work simulation with lock to ensure atomicity in-memory
            lock (_checkoutLock)
            {
                return CheckoutInternalAsync(cartId, ct).GetAwaiter().GetResult();
            }
        }

        private async Task<(Result result, OrderDTO? order)> CheckoutInternalAsync(Guid cartId, CancellationToken ct)
        {
            Cart cart = await _cartRepository.GetOrCreateAsync(cartId, ct);

            if (!cart.Items.Any()) 
                return (Result.Fail("Cart is empty", "CART_EMPTY"), null);

            // Validate product existence and stock (current)
            var productLookup = new Dictionary<Guid, Product>();
            foreach (var item in cart.Items)
            {
                Product? product = await _productRepository.GetAsync(item.ProductId, ct);
                if (product == null) 
                    return (Result.Fail($"Product {item.ProductId} not found", "PRODUCT_NOT_FOUND"), null);

                productLookup[item.ProductId] = product;

                if (item.Quantity > product.Stock) 
                    return (Result.Fail("Requested quantity exceeds available stock", "STOCK_EXCEEDED"), null);
            }

            // Pricing
            decimal subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);
            decimal discount = 0m;
            if (!string.IsNullOrWhiteSpace(cart.CouponCode))
            {
                var strategy = _couponFactory.GetStrategy(cart.CouponCode!);
                if (strategy != null && strategy.IsApplicable(subtotal))
                {
                    discount = strategy.CalculateDiscount(subtotal);
                }
            }

            decimal taxable = subtotal - discount;
            const decimal TaxRate = 0.18m;
            decimal tax = Math.Round(taxable * TaxRate, 2);
            decimal grandTotal = Math.Round(taxable + tax, 2);

            // Reduce stock (final re-check)
            foreach (var item in cart.Items)
            {
                var product = productLookup[item.ProductId];
                if (item.Quantity > product.Stock) 
                    return (Result.Fail("Stock changed, checkout aborted", "STOCK_CHANGED"), null);

                product.Stock -= item.Quantity;
                _productRepository.UpdateAsync(product, ct).GetAwaiter().GetResult();
            }

            // Create order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Items = cart.Items.Select(i => new CartItem { ProductId = i.ProductId, Quantity = i.Quantity, UnitPrice = i.UnitPrice }).ToList(),
                Subtotal = subtotal,
                Discount = discount,
                Tax = tax,
                GrandTotal = grandTotal
            };

            await _orderRepository.CreateAsync(order, ct);

            // Clear cart
            cart.Items.Clear();
            cart.CouponCode = null;
            await _cartRepository.UpdateAsync(cart, ct);

            OrderDTO orderDto = new OrderDTO
            {
                OrderId = order.Id,
                Items = order.Items.Select(i =>
                {
                    var p = productLookup[i.ProductId];
                    return new CartItemDTO
                    {
                        ProductId = i.ProductId,
                        ProductName = p.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    };
                }).ToList(),
                Subtotal = order.Subtotal,
                Discount = order.Discount,
                Tax = order.Tax,
                GrandTotal = order.GrandTotal,
                CreatedAt = order.CreatedAt
            };

			return (Result.Ok(cart), orderDto);

			//return ( Result.Ok(JsonSerializer.Serialize(cart)), orderDto);
		}

        public async Task<OrderDTO?> GetOrderAsync(Guid orderId, CancellationToken ct = default)
        {
            Order? order = await _orderRepository.GetAsync(orderId, ct);
            if (order == null) 
                return null;

            // Map products for names
            var items = new List<CartItemDTO>();

            foreach (var it in order.Items)
            {
                Product? p = await _productRepository.GetAsync(it.ProductId, ct);

                items.Add(new CartItemDTO
                {
                    ProductId = it.ProductId,
                    ProductName = p?.Name ?? "Unknown",
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice
                });
            }

            return new OrderDTO
            {
                OrderId = order.Id,
                Items = items,
                Subtotal = order.Subtotal,
                Discount = order.Discount,
                Tax = order.Tax,
                GrandTotal = order.GrandTotal,
                CreatedAt = order.CreatedAt
            };
        }

        private async Task<CartDTO> CalculationOnCartAsync(Cart cart, CancellationToken ct)
        {
            var products = (await _productRepository.GetAllAsync(ct)).ToDictionary(p => p.Id);

            var items = cart.Items.Select(i =>
            {
                var productName = products.TryGetValue(i.ProductId, out var p) ? p.Name : "Unknown";
                return new CartItemDTO
                {
                    ProductId = i.ProductId,
                    ProductName = productName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                };
            }).ToList();

            var subtotal = items.Sum(i => i.LineTotal);
            decimal discount = 0m;

            if (!string.IsNullOrWhiteSpace(cart.CouponCode))
            {
                ICouponStrategy? strategy = _couponFactory.GetStrategy(cart.CouponCode!);

                if (strategy != null && strategy.IsApplicable(subtotal))
                {
                    discount = strategy.CalculateDiscount(subtotal);
                }
            }

            const decimal TaxRate = 0.18m;
            var taxable = subtotal - discount;
            var tax = Math.Round(taxable * TaxRate, 2);
            var grandTotal = Math.Round(taxable + tax, 2);

            return new CartDTO
            {
                Id = cart.Id,
                Items = items,
                CouponCode = cart.CouponCode,
                Subtotal = subtotal,
                Discount = discount,
                Tax = tax,
                GrandTotal = grandTotal
            };
        }
    }
}

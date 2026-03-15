using SmartCart.Application.Interfaces;
using SmartCart.Domain.Entities;

namespace SmartCart.Infrastructure.Repositories
{
    public sealed class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public InMemoryProductRepository()
        {
            _products = new List<Product>
            {
                new() { Id = Guid.Parse("FCDA608A-2B59-40D9-BC3D-D35476279656"), Name = "Keyboard", Price = 1200m, Stock = 10 },
                new() { Id = Guid.Parse("69B2EAAE-C5F3-47E5-896E-C747DF212961"), Name = "Mouse", Price = 500m, Stock = 20 },
                new() { Id = Guid.Parse("466D5820-6467-4CEA-A664-D925DE587E0D"), Name = "USB Cable", Price = 150m, Stock = 50 },
                new() { Id = Guid.Parse("0774489A-488C-43CA-A4F8-D5EFFDF5C75A"), Name = "Pen Drive", Price = 1200m, Stock = 40 },
                new() { Id = Guid.Parse("53396AE8-445B-4059-8D1C-B51F9CA1DC48"), Name = "Monitor", Price = 500m, Stock = 60 },
                new() { Id = Guid.Parse("D853915C-40B2-4C3C-B32D-655022A73BC5"), Name = "HDMI Cable", Price = 150m, Stock = 80 },
            };
        }

        public Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default) =>
            Task.FromResult(_products.AsEnumerable());

        public Task<Product?> GetAsync(Guid id, CancellationToken ct = default) =>
            Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task UpdateAsync(Product product, CancellationToken ct = default)
        {
            // In-memory reference update is enough
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                existing.Price = product.Price;
                existing.Stock = product.Stock;
            }
            return Task.CompletedTask;
        }
    }
}

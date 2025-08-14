using Microsoft.EntityFrameworkCore;
using StarMarathon.Application.Abstractions;
using StarMarathon.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StarMarathon.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly StarDbContext _db;
    public ProductRepository(StarDbContext db) => _db = db;

    public Task<List<Product>> GetAllAsync(CancellationToken ct) => _db.Products.ToListAsync(ct);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Product product, CancellationToken ct) =>
        await _db.Products.AddAsync(product, ct);

    public Task RemoveAsync(Product product, CancellationToken ct)
    {
        _db.Products.Remove(product);
        return Task.CompletedTask;
    }
}

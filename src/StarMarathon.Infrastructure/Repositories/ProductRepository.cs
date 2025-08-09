using Microsoft.EntityFrameworkCore;
using StarMarathon.Application.Abstractions;
using StarMarathon.Domain.Entities;

namespace StarMarathon.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly StarDbContext _db;
    public ProductRepository(StarDbContext db) => _db = db;

    public Task<List<Product>> GetAllAsync(CancellationToken ct) =>
        _db.Products.ToListAsync(ct);
}

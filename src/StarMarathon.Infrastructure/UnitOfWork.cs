using StarMarathon.Application.Abstractions;

namespace StarMarathon.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly StarDbContext _db;
    public UnitOfWork(StarDbContext db) => _db = db;

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

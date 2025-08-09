using Microsoft.EntityFrameworkCore;
using StarMarathon.Application.Abstractions;
using StarMarathon.Domain.Entities;

namespace StarMarathon.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly StarDbContext _db;
    public EmployeeRepository(StarDbContext db) => _db = db;

    public Task<Employee?> GetByTgIdAsync(long tgId, CancellationToken ct) =>
        _db.Employees
           .Include(e => e.Groups)
           .FirstOrDefaultAsync(e => e.TgId == tgId, ct);
}

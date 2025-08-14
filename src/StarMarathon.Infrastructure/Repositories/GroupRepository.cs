using Microsoft.EntityFrameworkCore;
using StarMarathon.Application.Abstractions;
using StarMarathon.Domain.Entities;

namespace StarMarathon.Infrastructure.Repositories;

public sealed class GroupRepository : IGroupRepository
{
    private readonly StarDbContext _db;
    public GroupRepository(StarDbContext db) => _db = db;

    public Task<List<Group>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken ct)
    {
        var set = codes.Select(c => c.ToLowerInvariant()).ToHashSet();
        return _db.Groups.Where(g => set.Contains(g.Code.ToLower())).ToListAsync(ct);
    }

    public async Task<Group> GetOrCreateAsync(string code, string name, CancellationToken ct)
    {
        var g = await _db.Groups.FirstOrDefaultAsync(x => x.Code.ToLower() == code.ToLower(), ct);
        if (g is not null) return g;
        g = new Group(code, name);
        await _db.Groups.AddAsync(g, ct);
        return g;
    }
}

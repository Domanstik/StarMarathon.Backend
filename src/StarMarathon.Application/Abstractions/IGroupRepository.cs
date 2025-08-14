using StarMarathon.Domain.Entities;

namespace StarMarathon.Application.Abstractions;

public interface IGroupRepository
{
    Task<List<Group>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken ct);
    Task<Group> GetOrCreateAsync(string code, string name, CancellationToken ct);
}

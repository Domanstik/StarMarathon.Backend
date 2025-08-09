using StarMarathon.Domain.Entities;

namespace StarMarathon.Application.Abstractions;

public interface IEmployeeRepository
{
    Task<Employee?> GetByTgIdAsync(long tgId, CancellationToken ct);
}

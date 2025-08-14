using StarMarathon.Domain.Entities;
using DomainTask = StarMarathon.Domain.Entities.Task;
using Task = System.Threading.Tasks.Task;

namespace StarMarathon.Application.Abstractions;

public interface ITaskRepository
{
    Task<List<DomainTask>> GetByLanguageAndGroupsAsync(string language, IEnumerable<Guid> groupIds, CancellationToken ct);
    Task<EmployeeTaskStatus?> FindStatusAsync(Guid employeeId, Guid taskId, CancellationToken ct);
    Task AddStatusAsync(EmployeeTaskStatus status, CancellationToken ct);

    Task<List<DomainTask>> ListAsync(CancellationToken ct);
    Task<DomainTask?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(DomainTask task, CancellationToken ct);
    Task RemoveAsync(DomainTask task, CancellationToken ct);
}

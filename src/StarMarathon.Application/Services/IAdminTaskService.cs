using StarMarathon.Application.DTOs.Admin;

namespace StarMarathon.Application.Services;

public interface IAdminTaskService
{
    Task<IReadOnlyList<TaskAdminResponse>> ListAsync(CancellationToken ct);
    Task<TaskAdminResponse> GetAsync(Guid id, CancellationToken ct);
    Task<Guid> CreateAsync(TaskAdminCreateRequest req, CancellationToken ct);
    Task UpdateAsync(Guid id, TaskAdminUpdateRequest req, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

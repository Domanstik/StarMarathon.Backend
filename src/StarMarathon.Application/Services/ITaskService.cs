using StarMarathon.Application.DTOs;

namespace StarMarathon.Application.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskDto>> GetAvailableAsync(long tgId, CancellationToken ct);
    Task CompleteAsync(Guid taskId, long tgId, CancellationToken ct);
}

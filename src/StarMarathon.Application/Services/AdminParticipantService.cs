using StarMarathon.Application.Abstractions;
using StarMarathon.Application.DTOs;

namespace StarMarathon.Application.Services;

public interface IAdminParticipantService
{
    Task<IReadOnlyList<ParticipantResponse>> GetAsync(Guid taskId, CancellationToken ct);
}

public sealed class AdminParticipantService : IAdminParticipantService
{
    private readonly IParticipantRepository _participants;
    private readonly ITaskRepository _tasks;

    public AdminParticipantService(IParticipantRepository participants, ITaskRepository tasks)
    {
        _participants = participants; _tasks = tasks;
    }

    public async Task<IReadOnlyList<ParticipantResponse>> GetAsync(Guid taskId, CancellationToken ct)
    {
        var task = await _tasks.GetByIdAsync(taskId, ct);
        if (task is null) throw new KeyNotFoundException("Task not found");
        return await _participants.GetListForTaskAsync(taskId, ct);
    }
}

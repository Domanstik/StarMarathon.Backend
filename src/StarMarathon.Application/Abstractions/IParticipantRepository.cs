using StarMarathon.Application.DTOs;
using StarMarathon.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StarMarathon.Application.Abstractions;

public interface IParticipantRepository
{
    Task<bool> ExistsAsync(Guid taskId, Guid employeeId, CancellationToken ct);
    Task AddAsync(TaskParticipant participant, CancellationToken ct);
    Task<List<ParticipantResponse>> GetListForTaskAsync(Guid taskId, CancellationToken ct);
}

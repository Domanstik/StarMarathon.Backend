using StarMarathon.Application.Abstractions;
using StarMarathon.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StarMarathon.Application.Services;

public sealed class UserParticipationService : IUserParticipationService
{
    private readonly IEmployeeRepository _employees;
    private readonly ITaskRepository _tasks;
    private readonly IParticipantRepository _participants;
    private readonly IUnitOfWork _uow;

    public UserParticipationService(IEmployeeRepository employees, ITaskRepository tasks,
        IParticipantRepository participants, IUnitOfWork uow)
    {
        _employees = employees; _tasks = tasks; _participants = participants; _uow = uow;
    }

    public async Task JoinAsync(long tgId, Guid taskId, CancellationToken ct)
    {
        var emp = await _employees.GetByTgIdAsync(tgId, ct) ?? throw new KeyNotFoundException("Employee not found");
        var task = await _tasks.GetByIdAsync(taskId, ct) ?? throw new KeyNotFoundException("Task not found");

        // идемпотентно
        if (await _participants.ExistsAsync(taskId, emp.Id, ct)) return;

        await _participants.AddAsync(new TaskParticipant(taskId, emp.Id), ct);
        await _uow.SaveChangesAsync(ct);
    }
}

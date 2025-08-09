using StarMarathon.Application.Abstractions;
using StarMarathon.Application.DTOs;
using DomainTask = StarMarathon.Domain.Entities.Task;

namespace StarMarathon.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly IEmployeeRepository _employees;
    private readonly ITaskRepository _tasks;
    private readonly IUnitOfWork _uow;

    public TaskService(IEmployeeRepository employees, ITaskRepository tasks, IUnitOfWork uow)
    {
        _employees = employees;
        _tasks = tasks;
        _uow = uow;
    }

    public async Task<IReadOnlyList<TaskDto>> GetAvailableAsync(long tgId, CancellationToken ct)
    {
        var emp = await _employees.GetByTgIdAsync(tgId, ct)
                  ?? throw new KeyNotFoundException("Employee not found");

        var groupIds = emp.Groups.Select(g => g.GroupId).ToArray();

        var tasks = await _tasks.GetByLanguageAndGroupsAsync(emp.Language, groupIds, ct);

        var result = tasks.Select(t =>
            new TaskDto(
                t.Id, t.Name, t.Description, t.End, t.WinningReward, t.ParticipationReward,
                t.EmployeeStatuses.Any(s => s.EmployeeId == emp.Id && s.IsCompleted)
            )).ToList();

        return result;
    }

    public async Task CompleteAsync(Guid taskId, long tgId, CancellationToken ct)
    {
        var emp = await _employees.GetByTgIdAsync(tgId, ct)
                  ?? throw new KeyNotFoundException("Employee not found");

        var status = await _tasks.FindStatusAsync(emp.Id, taskId, ct);
        if (status is null)
        {
            await _tasks.AddStatusAsync(new()
            {
                EmployeeId = emp.Id,
                TaskId = taskId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            }, ct);
        }
        else if (!status.IsCompleted)
        {
            status.IsCompleted = true;
            status.CompletedAt = DateTime.UtcNow;
        }

        await _uow.SaveChangesAsync(ct);
    }
}

using Microsoft.EntityFrameworkCore;
using StarMarathon.Application.Abstractions;
using StarMarathon.Domain.Entities;
using DomainTask = StarMarathon.Domain.Entities.Task;
using Task = System.Threading.Tasks.Task;

namespace StarMarathon.Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly StarDbContext _db;
    public TaskRepository(StarDbContext db) => _db = db;

    // user-facing
    public Task<List<DomainTask>> GetByLanguageAndGroupsAsync(string language, IEnumerable<Guid> groupIds, CancellationToken ct) =>
        _db.Tasks
           .Include(t => t.TaskGroups).ThenInclude(tg => tg.Group)
           .Include(t => t.EmployeeStatuses)
           .Where(t => t.Language == language && t.TaskGroups.Any(g => groupIds.Contains(g.GroupId)))
           .ToListAsync(ct);

    public Task<EmployeeTaskStatus?> FindStatusAsync(Guid employeeId, Guid taskId, CancellationToken ct) =>
        _db.EmployeeTaskStatuses.FindAsync(new object[] { employeeId, taskId }, ct).AsTask();

    public async Task AddStatusAsync(EmployeeTaskStatus status, CancellationToken ct) =>
        await _db.EmployeeTaskStatuses.AddAsync(status, ct);

    // admin
    public Task<List<DomainTask>> ListAsync(CancellationToken ct) =>
        _db.Tasks.Include(t => t.TaskGroups).ThenInclude(tg => tg.Group).ToListAsync(ct);

    public Task<DomainTask?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Tasks.Include(t => t.TaskGroups).ThenInclude(tg => tg.Group)
                 .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task AddAsync(DomainTask task, CancellationToken ct) =>
        await _db.Tasks.AddAsync(task, ct);

    public Task RemoveAsync(DomainTask task, CancellationToken ct)
    {
        _db.Tasks.Remove(task);
        return Task.CompletedTask;
    }
}

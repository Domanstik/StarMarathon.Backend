using Microsoft.EntityFrameworkCore;
using StarMarathon.Application.Abstractions;
using StarMarathon.Application.DTOs;
using StarMarathon.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StarMarathon.Infrastructure.Repositories;

public sealed class ParticipantRepository : IParticipantRepository
{
    private readonly StarDbContext _db;
    public ParticipantRepository(StarDbContext db) => _db = db;

    public Task<bool> ExistsAsync(Guid taskId, Guid employeeId, CancellationToken ct) =>
        _db.TaskParticipants.AnyAsync(p => p.TaskId == taskId && p.EmployeeId == employeeId, ct);

    public async Task AddAsync(TaskParticipant participant, CancellationToken ct) =>
        await _db.TaskParticipants.AddAsync(participant, ct);

    // Возвращаем сразу удобный список для админа: tgId, ФИО/username, дата, статус выполнения
    public async Task<List<ParticipantResponse>> GetListForTaskAsync(Guid taskId, CancellationToken ct)
    {
        var q =
            from p in _db.TaskParticipants
            where p.TaskId == taskId
            join e in _db.Employees on p.EmployeeId equals e.Id
            join s in _db.EmployeeTaskStatuses.Where(s => s.TaskId == taskId)
                on e.Id equals s.EmployeeId into sts
            from s in sts.DefaultIfEmpty()
            orderby p.JoinedAt
            select new ParticipantResponse(
                e.TgId,
                p.JoinedAt,
                s != null && s.IsCompleted
            );


        return await q.ToListAsync(ct);
    }
}

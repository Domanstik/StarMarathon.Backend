using StarMarathon.Application.Abstractions;
using StarMarathon.Application.DTOs.Admin;
using StarMarathon.Domain.Entities;
using DomainTask = StarMarathon.Domain.Entities.Task;
using Task = System.Threading.Tasks.Task;

namespace StarMarathon.Application.Services;

public sealed class AdminTaskService : IAdminTaskService
{
    private readonly ITaskRepository _tasks;
    private readonly IGroupRepository _groups;
    private readonly IUnitOfWork _uow;

    public AdminTaskService(ITaskRepository tasks, IGroupRepository groups, IUnitOfWork uow)
    {
        _tasks = tasks; _groups = groups; _uow = uow;
    }

    public async Task<IReadOnlyList<TaskAdminResponse>> ListAsync(CancellationToken ct)
    {
        var list = await _tasks.ListAsync(ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task<TaskAdminResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var entity = await _tasks.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Task not found");
        return ToResponse(entity);
    }

    public async Task<Guid> CreateAsync(TaskAdminCreateRequest req, CancellationToken ct)
    {
        Validate(req.Name, req.Description, req.Language, req.WinningReward, req.ParticipationReward);

        var entity = new DomainTask(req.Name, req.Description, req.Language, req.WinningReward, req.ParticipationReward, req.End);

        // привязка к группам
        if (req.GroupCodes is { Count: > 0 })
        {
            var groups = await _groups.GetByCodesAsync(req.GroupCodes, ct);
            var known = new HashSet<string>(groups.Select(g => g.Code));
            // создадим недостающие группы на лету (можно поменять на "ошибку, если нет")
            foreach (var missing in req.GroupCodes.Where(c => !known.Contains(c)))
                groups.Add(await _groups.GetOrCreateAsync(missing, missing, ct));

            foreach (var g in groups)
                entity.TaskGroups.Add(new TaskGroup { Task = entity, Group = g });
        }

        await _tasks.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(Guid id, TaskAdminUpdateRequest req, CancellationToken ct)
    {
        Validate(req.Name, req.Description, req.Language, req.WinningReward, req.ParticipationReward);

        var entity = await _tasks.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Task not found");

        entity.Update(req.Name, req.Description, req.Language,
                      req.WinningReward, req.ParticipationReward, req.End);

        if (req.GroupCodes is not null)
        {
            var desiredCodes = req.GroupCodes
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var groups = await _groups.GetByCodesAsync(desiredCodes, ct);
            var have = new HashSet<string>(groups.Select(g => g.Code), StringComparer.OrdinalIgnoreCase);
            foreach (var code in desiredCodes)
                if (!have.Contains(code))
                    groups.Add(await _groups.GetOrCreateAsync(code, code, ct));

            var desiredIds = groups.Select(g => g.Id).ToHashSet();

            foreach (var tg in entity.TaskGroups.Where(tg => !desiredIds.Contains(tg.GroupId)).ToList())
                entity.TaskGroups.Remove(tg);

            var currentIds = entity.TaskGroups.Select(tg => tg.GroupId).ToHashSet();
            foreach (var g in groups)
                if (!currentIds.Contains(g.Id))
                    entity.TaskGroups.Add(new TaskGroup { TaskId = entity.Id, GroupId = g.Id });
        }

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _tasks.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Task not found");
        await _tasks.RemoveAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static void Validate(string name, string descr, string lang, int win, int part)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(descr)) throw new ArgumentException("Description is required");
        if (string.IsNullOrWhiteSpace(lang)) throw new ArgumentException("Language is required");
        if (win < 0 || part < 0) throw new ArgumentException("Rewards must be >= 0");
    }

    private static TaskAdminResponse ToResponse(DomainTask t) =>
        new(t.Id, t.Name, t.Description, t.Language, t.WinningReward, t.ParticipationReward, t.End,
            t.TaskGroups.Select(x => x.Group.Code).Distinct().ToList());
}

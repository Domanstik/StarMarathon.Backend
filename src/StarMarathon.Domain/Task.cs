namespace StarMarathon.Domain.Entities;

public class Task : Entity<Guid>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Language { get; private set; } = "ru";
    public int WinningReward { get; private set; }
    public int ParticipationReward { get; private set; }
    public DateTime Created { get; private set; }
    public DateTime End { get; private set; }
    public ICollection<TaskGroup> TaskGroups { get; } = new List<TaskGroup>();
    public ICollection<EmployeeTaskStatus> EmployeeStatuses { get; } = new List<EmployeeTaskStatus>();
    private Task() { }
    public Task(string name, string descr, string lang, int win, int part, DateTime end)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = descr;
        Language = lang;
        WinningReward = win;
        ParticipationReward = part;
        Created = DateTime.UtcNow;
        End = end;
    }
}

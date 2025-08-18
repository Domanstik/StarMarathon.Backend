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

    public Task(string name, string description, string language,
                int winReward, int partReward, DateTime end)
    {
        Id = Guid.NewGuid();
        Created = DateTime.UtcNow;
        Update(name, description, language, winReward, partReward, end);
    }

    public void Update(string name, string description, string language,
                       int winReward, int partReward, DateTime end)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required", nameof(description));
        if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Language is required", nameof(language));
        if (winReward < 0) throw new ArgumentOutOfRangeException(nameof(winReward), "Must be >= 0");
        if (partReward < 0) throw new ArgumentOutOfRangeException(nameof(partReward), "Must be >= 0");

        Name = name.Trim();
        Description = description.Trim();
        Language = language.Trim().ToLowerInvariant();
        WinningReward = winReward;
        ParticipationReward = partReward;
        End = end;
    }
}

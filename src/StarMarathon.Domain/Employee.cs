namespace StarMarathon.Domain.Entities;

public class Employee : Entity<Guid>
{
    public long TgId { get; private set; }
    public string? TgUsername { get; private set; }
    public string Phone { get; private set; } = default!;
    public string Language { get; private set; } = "ru";
    public string? AvatarUrl { get; private set; }
    public ICollection<EmployeeGroup> Groups { get; private set; } = new List<EmployeeGroup>();
    public ICollection<EmployeeTaskStatus> TaskStatuses { get; private set; } = new List<EmployeeTaskStatus>();
    private Employee() { }
    public Employee(long tgId, string phone, string language)
    {
        Id = Guid.NewGuid();
        TgId = tgId;
        Phone = phone;
        Language = language;
    }
}

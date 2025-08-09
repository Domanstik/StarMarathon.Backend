namespace StarMarathon.Domain.Entities;

public class Group : Entity<Guid>
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public ICollection<EmployeeGroup> EmployeeGroups { get; set; } = new List<EmployeeGroup>();
    public ICollection<TaskGroup> TaskGroups { get; set; } = new List<TaskGroup>();
    private Group() { }
    public Group(string code, string name)
    {
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
    }
}

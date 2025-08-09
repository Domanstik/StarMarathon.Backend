namespace StarMarathon.Domain.Entities;

public class EmployeeGroup
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = default!;
}

namespace StarMarathon.Domain.Entities;

public class EmployeeTaskStatus
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    public Guid TaskId { get; set; }
    public Task Task { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

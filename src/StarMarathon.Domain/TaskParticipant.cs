namespace StarMarathon.Domain.Entities;

public class TaskParticipant : Entity<Guid>
{
    public Guid TaskId { get; private set; }
    public Task Task { get; private set; } = default!;

    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = default!;

    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

    private TaskParticipant() { }

    public TaskParticipant(Guid taskId, Guid employeeId)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        EmployeeId = employeeId;
        JoinedAt = DateTime.UtcNow;
    }
}

namespace StarMarathon.Domain.Entities;

public class Notification : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public string Description { get; private set; } = default!;
    public bool IsRead { get; private set; }
    public DateTime Created { get; private set; }
    private Notification() { }
    public Notification(Guid e, string d)
    {
        Id = Guid.NewGuid();
        EmployeeId = e;
        Description = d;
        Created = DateTime.UtcNow;
    }
    public void MarkRead() => IsRead = true;
}

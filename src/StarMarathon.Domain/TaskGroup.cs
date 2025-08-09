namespace StarMarathon.Domain.Entities;

public class TaskGroup
{
    public Guid TaskId { get; set; }
    public Task Task { get; set; } = default!;
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = default!;
}

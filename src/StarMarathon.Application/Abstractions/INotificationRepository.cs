using StarMarathon.Domain.Entities;

namespace StarMarathon.Application.Abstractions;

public interface INotificationRepository
{
    Task<List<Notification>> GetForEmployeeAsync(Guid employeeId, CancellationToken ct);
}

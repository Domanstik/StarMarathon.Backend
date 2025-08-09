using StarMarathon.Application.DTOs;

namespace StarMarathon.Application.Services;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetAsync(long tgId, CancellationToken ct);
}

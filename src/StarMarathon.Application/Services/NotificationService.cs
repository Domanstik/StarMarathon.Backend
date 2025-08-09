using StarMarathon.Application.Abstractions;
using StarMarathon.Application.DTOs;

namespace StarMarathon.Application.Services;

public sealed class NotificationService : INotificationService
{
    private readonly IEmployeeRepository _employees;
    private readonly INotificationRepository _notifications;

    public NotificationService(IEmployeeRepository employees, INotificationRepository notifications)
    {
        _employees = employees;
        _notifications = notifications;
    }

    public async Task<IReadOnlyList<NotificationDto>> GetAsync(long tgId, CancellationToken ct)
    {
        var emp = await _employees.GetByTgIdAsync(tgId, ct)
                  ?? throw new KeyNotFoundException("Employee not found");

        var list = await _notifications.GetForEmployeeAsync(emp.Id, ct);
        return list
            .OrderByDescending(n => n.Created)
            .Select(n => new NotificationDto(n.Id, n.Description, n.IsRead, n.Created))
            .ToList();
    }
}

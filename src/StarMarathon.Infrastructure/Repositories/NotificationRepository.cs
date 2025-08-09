using Microsoft.EntityFrameworkCore;
using StarMarathon.Application.Abstractions;
using StarMarathon.Domain.Entities;

namespace StarMarathon.Infrastructure.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly StarDbContext _db;
    public NotificationRepository(StarDbContext db) => _db = db;

    public Task<List<Notification>> GetForEmployeeAsync(Guid employeeId, CancellationToken ct) =>
        _db.Notifications.Where(n => n.EmployeeId == employeeId).ToListAsync(ct);
}

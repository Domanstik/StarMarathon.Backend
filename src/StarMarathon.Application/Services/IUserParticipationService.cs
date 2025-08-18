namespace StarMarathon.Application.Services;

public interface IUserParticipationService
{
    Task JoinAsync(long tgId, Guid taskId, CancellationToken ct);
}

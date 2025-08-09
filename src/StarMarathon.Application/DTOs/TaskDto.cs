namespace StarMarathon.Application.DTOs;

public record TaskDto(
    Guid Id,
    string Name,
    string Description,
    DateTime End,
    int WinningReward,
    int ParticipationReward,
    bool IsCompleted
);

namespace StarMarathon.Application.DTOs.Admin;

public record TaskAdminCreateRequest(
    string Name,
    string Description,
    string Language,
    int WinningReward,
    int ParticipationReward,
    DateTime End,
    List<string>? GroupCodes // например: ["all", "hr"]
);

public record TaskAdminUpdateRequest(
    string Name,
    string Description,
    string Language,
    int WinningReward,
    int ParticipationReward,
    DateTime End,
    List<string>? GroupCodes
);

public record TaskAdminResponse(
    Guid Id,
    string Name,
    string Description,
    string Language,
    int WinningReward,
    int ParticipationReward,
    DateTime End,
    List<string> GroupCodes
);

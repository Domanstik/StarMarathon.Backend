namespace StarMarathon.Application.DTOs;

public record NotificationDto(
    Guid Id,
    string Description,
    bool IsRead,
    DateTime Created
);

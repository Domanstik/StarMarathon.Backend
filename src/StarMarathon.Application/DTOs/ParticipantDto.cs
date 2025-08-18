namespace StarMarathon.Application.DTOs;

public record ParticipantResponse(long TgId, DateTime JoinedAt, bool IsCompleted);

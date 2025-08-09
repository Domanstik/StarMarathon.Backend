namespace StarMarathon.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    int Price,
    string ImageUrl
);

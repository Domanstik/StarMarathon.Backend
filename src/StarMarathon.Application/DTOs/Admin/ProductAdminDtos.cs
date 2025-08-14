namespace StarMarathon.Application.DTOs.Admin;

public record ProductAdminCreateRequest(
    string Name,
    string Description,
    int Price,
    string ImageUrl
);

public record ProductAdminUpdateRequest(
    string Name,
    string Description,
    int Price,
    string ImageUrl
);

public record ProductAdminResponse(
    Guid Id,
    string Name,
    string Description,
    int Price,
    string ImageUrl
);

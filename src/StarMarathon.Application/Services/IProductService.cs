using StarMarathon.Application.DTOs;

namespace StarMarathon.Application.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAsync(CancellationToken ct);
}

using StarMarathon.Application.DTOs.Admin;

namespace StarMarathon.Application.Services;

public interface IAdminProductService
{
    Task<IReadOnlyList<ProductAdminResponse>> ListAsync(CancellationToken ct);
    Task<ProductAdminResponse> GetAsync(Guid id, CancellationToken ct);
    Task<Guid> CreateAsync(ProductAdminCreateRequest req, CancellationToken ct);
    Task UpdateAsync(Guid id, ProductAdminUpdateRequest req, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

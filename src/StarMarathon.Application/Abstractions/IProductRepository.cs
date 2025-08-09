using StarMarathon.Domain.Entities;

namespace StarMarathon.Application.Abstractions;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken ct);
}

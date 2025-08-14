using StarMarathon.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StarMarathon.Application.Abstractions;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken ct);

    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task RemoveAsync(Product product, CancellationToken ct);
}

using StarMarathon.Application.Abstractions;
using StarMarathon.Application.DTOs;

namespace StarMarathon.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _products;

    public ProductService(IProductRepository products) => _products = products;

    public async Task<IReadOnlyList<ProductDto>> GetAsync(CancellationToken ct)
    {
        var list = await _products.GetAllAsync(ct);
        return list.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl)).ToList();
    }
}

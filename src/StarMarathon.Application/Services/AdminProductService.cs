using StarMarathon.Application.Abstractions;
using StarMarathon.Application.DTOs.Admin;
using StarMarathon.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StarMarathon.Application.Services;

public sealed class AdminProductService : IAdminProductService
{
    private readonly IProductRepository _products;
    private readonly IUnitOfWork _uow;

    public AdminProductService(IProductRepository products, IUnitOfWork uow)
    {
        _products = products; _uow = uow;
    }

    public async Task<IReadOnlyList<ProductAdminResponse>> ListAsync(CancellationToken ct)
    {
        var list = await _products.GetAllAsync(ct);
        return list.Select(ToResp).ToList();
    }

    public async Task<ProductAdminResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var p = await _products.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Product not found");
        return ToResp(p);
    }

    public async Task<Guid> CreateAsync(ProductAdminCreateRequest req, CancellationToken ct)
    {
        Validate(req.Name, req.Description, req.Price, req.ImageUrl);
        var p = new Product(req.Name, req.Description, req.Price, req.ImageUrl);
        await _products.AddAsync(p, ct);
        await _uow.SaveChangesAsync(ct);
        return p.Id;
    }

    public async Task UpdateAsync(Guid id, ProductAdminUpdateRequest req, CancellationToken ct)
    {
        Validate(req.Name, req.Description, req.Price, req.ImageUrl);
        var p = await _products.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Product not found");

        p.GetType().GetProperty(nameof(Product.Name))!.SetValue(p, req.Name);
        p.GetType().GetProperty(nameof(Product.Description))!.SetValue(p, req.Description);
        p.GetType().GetProperty(nameof(Product.Price))!.SetValue(p, req.Price);
        p.GetType().GetProperty(nameof(Product.ImageUrl))!.SetValue(p, req.ImageUrl);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var p = await _products.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Product not found");
        await _products.RemoveAsync(p, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static void Validate(string name, string descr, int price, string image)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(descr)) throw new ArgumentException("Description is required");
        if (price < 0) throw new ArgumentException("Price must be >= 0");
        if (string.IsNullOrWhiteSpace(image)) throw new ArgumentException("ImageUrl is required");
    }

    private static ProductAdminResponse ToResp(Product p) =>
        new(p.Id, p.Name, p.Description, p.Price, p.ImageUrl);
}

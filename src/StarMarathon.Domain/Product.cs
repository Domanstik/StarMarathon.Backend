namespace StarMarathon.Domain.Entities;

public class Product : Entity<Guid>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int Price { get; private set; }
    public string ImageUrl { get; private set; } = default!;
    private Product() { }
    public Product(string n, string d, int p, string img)
    {
        Id = Guid.NewGuid();
        Name = n;
        Description = d;
        Price = p;
        ImageUrl = img;
    }
}

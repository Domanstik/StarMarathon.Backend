namespace StarMarathon.Domain.Entities;

public class Product : Entity<Guid>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int Price { get; private set; }
    public string ImageUrl { get; private set; } = default!;

    private Product() { }

    public Product(string name, string description, int price, string imageUrl)
    {
        Id = Guid.NewGuid();
        Update(name, description, price, imageUrl);
    }

    public void Update(string name, string description, int price, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required", nameof(description));
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Must be >= 0");
        if (string.IsNullOrWhiteSpace(imageUrl)) throw new ArgumentException("ImageUrl is required", nameof(imageUrl));

        Name = name.Trim();
        Description = description.Trim();
        Price = price;
        ImageUrl = imageUrl.Trim();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StarMarathon.Infrastructure;

public sealed class StarDbContextFactory : IDesignTimeDbContextFactory<StarDbContext>
{
    public StarDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                 ?? "Host=localhost;Port=5433;Database=star_db;Username=star;Password=star";

        var opts = new DbContextOptionsBuilder<StarDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new StarDbContext(opts);
    }
}

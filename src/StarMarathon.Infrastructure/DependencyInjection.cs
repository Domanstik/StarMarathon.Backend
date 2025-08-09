using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarMarathon.Application.Abstractions;
using StarMarathon.Infrastructure.Repositories;

namespace StarMarathon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<StarDbContext>(o => o.UseNpgsql(cfg.GetConnectionString("Default")));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}

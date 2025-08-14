using Microsoft.Extensions.DependencyInjection;
using StarMarathon.Application.Services;

namespace StarMarathon.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IProductService, ProductService>();

        services.AddScoped<IAdminTaskService, AdminTaskService>();
        services.AddScoped<IAdminProductService, AdminProductService>();
        return services;
    }
}

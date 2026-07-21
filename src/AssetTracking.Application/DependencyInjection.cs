using AssetTracking.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AssetTracking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IAssetService, AssetService>();

        return services;
    }
}

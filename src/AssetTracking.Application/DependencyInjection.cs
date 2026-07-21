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
        services.AddScoped<IAssetCategoryService, AssetCategoryService>();
        services.AddScoped<IAssetBrandService, AssetBrandService>();
        services.AddScoped<IAssetModelService, AssetModelService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IMovementService, MovementService>();
        services.AddScoped<IAccessControlService, AccessControlService>();
        services.AddScoped<IAlertService, AlertService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}

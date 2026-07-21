using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Interfaces;
using AssetTracking.Infrastructure.Persistence;
using AssetTracking.Infrastructure.Repositories;
using AssetTracking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace AssetTracking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IAssetMovementRepository, AssetMovementRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IReportExportService, ReportExportService>();

        return services;
    }

    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await context.Database.EnsureCreatedAsync();

        if (await context.Roles.AnyAsync()) return;

        var adminRole = new Role
        {
            Name = "Administrador",
            Description = "Acceso completo al sistema"
        };

        var permissions = new[]
        {
            new Permission { Code = "users.read", Name = "Ver usuarios", Module = "Seguridad" },
            new Permission { Code = "users.write", Name = "Gestionar usuarios", Module = "Seguridad" }
        };

        context.Permissions.AddRange(permissions);
        context.Roles.Add(adminRole);
        await context.SaveChangesAsync();

        foreach (var permission in permissions)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id
            });
        }

        context.Users.Add(new User
        {
            Email = "admin@institucion.edu",
            FullName = "Administrador del Sistema",
            RoleId = adminRole.Id,
            PasswordHash = passwordHasher.Hash("Admin123!")
        });

        context.AssetCategories.AddRange(
            new AssetCategory { Name = "Laptop", Description = "Computadoras portátiles" },
            new AssetCategory { Name = "Tablet", Description = "Tabletas" },
            new AssetCategory { Name = "Cámara", Description = "Cámaras fotográficas y de video" },
            new AssetCategory { Name = "Proyector", Description = "Proyectores multimedia" },
            new AssetCategory { Name = "Monitor", Description = "Monitores y pantallas" },
            new AssetCategory { Name = "Impresora", Description = "Impresoras" },
            new AssetCategory { Name = "Otros", Description = "Otros activos tecnológicos" });

        var mainCampus = new Location
        {
            Name = "Campus Principal",
            Code = "CAMPUS-01",
            LocationType = Domain.Enums.LocationType.Campus
        };
        context.Locations.Add(mainCampus);
        await context.SaveChangesAsync();

        context.Locations.AddRange(
            new Location { Name = "Puerta Norte", Code = "PUERTA-N", LocationType = Domain.Enums.LocationType.Gate, ParentLocationId = mainCampus.Id },
            new Location { Name = "Puerta Sur", Code = "PUERTA-S", LocationType = Domain.Enums.LocationType.Gate, ParentLocationId = mainCampus.Id },
            new Location { Name = "Biblioteca Central", Code = "BIBLIO-01", LocationType = Domain.Enums.LocationType.Library, ParentLocationId = mainCampus.Id });

        await context.SaveChangesAsync();
    }
}

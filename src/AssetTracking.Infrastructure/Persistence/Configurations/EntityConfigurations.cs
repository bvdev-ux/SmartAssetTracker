using AssetTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetTracking.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.FullName).HasMaxLength(200).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
        builder.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(r => r.Name).IsUnique();
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Code).HasMaxLength(100).IsRequired();
        builder.HasIndex(p => p.Code).IsUnique();
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");
        builder.HasKey(rp => rp.Id);
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();
        builder.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId);
        builder.HasOne(rp => rp.Permission).WithMany(p => p.RolePermissions).HasForeignKey(rp => rp.PermissionId);
    }
}

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("people");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.DocumentNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(p => p.DocumentNumber).IsUnique();
        builder.Property(p => p.UniversityCode).HasMaxLength(50);
        builder.HasIndex(p => p.UniversityCode).IsUnique();
        builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Career).HasMaxLength(150);
        builder.Property(p => p.Faculty).HasMaxLength(150);
        builder.Property(p => p.CardQrCode).HasMaxLength(256);
        builder.HasIndex(p => p.CardQrCode).IsUnique();
    }
}

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("assets");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AssetTag).HasMaxLength(50).IsRequired();
        builder.HasIndex(a => a.AssetTag).IsUnique();
        builder.Property(a => a.SerialNumber).HasMaxLength(100);
        builder.HasIndex(a => a.SerialNumber).IsUnique();
        builder.Property(a => a.QrCode).HasMaxLength(256);
        builder.HasIndex(a => a.QrCode).IsUnique();
        builder.Property(a => a.RfidTag).HasMaxLength(256);
        builder.Property(a => a.Color).HasMaxLength(50);
        builder.Property(a => a.Features).HasMaxLength(1000);
        builder.Property(a => a.PhotoUrl).HasMaxLength(500);
        builder.HasOne(a => a.Category).WithMany(c => c.Assets).HasForeignKey(a => a.CategoryId);
        builder.HasOne(a => a.Model).WithMany(m => m.Assets).HasForeignKey(a => a.ModelId);
        builder.HasOne(a => a.Owner).WithMany(p => p.OwnedAssets).HasForeignKey(a => a.OwnerId);
        builder.HasOne(a => a.CurrentLocation).WithMany(l => l.Assets).HasForeignKey(a => a.CurrentLocationId);
    }
}

public class AssetCategoryConfiguration : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.ToTable("asset_categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(c => c.Name).IsUnique();
    }
}

public class AssetBrandConfiguration : IEntityTypeConfiguration<AssetBrand>
{
    public void Configure(EntityTypeBuilder<AssetBrand> builder)
    {
        builder.ToTable("asset_brands");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(b => b.Name).IsUnique();
    }
}

public class AssetModelConfiguration : IEntityTypeConfiguration<AssetModel>
{
    public void Configure(EntityTypeBuilder<AssetModel> builder)
    {
        builder.ToTable("asset_models");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).HasMaxLength(100).IsRequired();
        builder.HasOne(m => m.Brand).WithMany(b => b.Models).HasForeignKey(m => m.BrandId);
        builder.HasIndex(m => new { m.BrandId, m.Name }).IsUnique();
    }
}

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).HasMaxLength(150).IsRequired();
        builder.Property(l => l.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(l => l.Code).IsUnique();
        builder.HasOne(l => l.ParentLocation).WithMany(l => l.ChildLocations).HasForeignKey(l => l.ParentLocationId);
    }
}

public class AssetMovementConfiguration : IEntityTypeConfiguration<AssetMovement>
{
    public void Configure(EntityTypeBuilder<AssetMovement> builder)
    {
        builder.ToTable("asset_movements");
        builder.HasKey(m => m.Id);
        builder.HasOne(m => m.Asset).WithMany(a => a.Movements).HasForeignKey(m => m.AssetId);
        builder.HasOne(m => m.Person).WithMany(p => p.Movements).HasForeignKey(m => m.PersonId);
        builder.HasOne(m => m.Location).WithMany(l => l.Movements).HasForeignKey(m => m.LocationId);
        builder.HasIndex(m => m.OccurredAt);
    }
}

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("alerts");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Message).HasMaxLength(500).IsRequired();
        builder.HasOne(a => a.Asset).WithMany(asset => asset.Alerts).HasForeignKey(a => a.AssetId);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(a => a.UserAgent).HasMaxLength(500);
        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId);
        builder.HasIndex(a => a.OccurredAt);
    }
}

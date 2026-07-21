using AssetTracking.Domain.Common;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Domain.Entities;

public class AssetCategory : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Asset> Assets { get; set; } = [];
}

public class AssetBrand : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<AssetModel> Models { get; set; } = [];
}

public class AssetModel : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid BrandId { get; set; }
    public bool IsActive { get; set; } = true;

    public AssetBrand Brand { get; set; } = null!;
    public ICollection<Asset> Assets { get; set; } = [];
}

public class Asset : AuditableEntity
{
    public string AssetTag { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? QrCode { get; set; }
    public string? RfidTag { get; set; }
    public Guid CategoryId { get; set; }
    public Guid ModelId { get; set; }
    public string? Color { get; set; }
    public string? Features { get; set; }
    public string? PhotoUrl { get; set; }
    public Guid? OwnerId { get; set; }
    public AssetStatusType Status { get; set; } = AssetStatusType.Available;
    public Guid? CurrentLocationId { get; set; }
    public bool IsInsideCampus { get; set; } = true;
    public string? Notes { get; set; }

    public AssetCategory Category { get; set; } = null!;
    public AssetModel Model { get; set; } = null!;
    public Person? Owner { get; set; }
    public Location? CurrentLocation { get; set; }
    public ICollection<AssetMovement> Movements { get; set; } = [];
    public ICollection<Alert> Alerts { get; set; } = [];
}

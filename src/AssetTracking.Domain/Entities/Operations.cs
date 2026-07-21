using AssetTracking.Domain.Common;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Domain.Entities;

public class Location : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public LocationType LocationType { get; set; }
    public Guid? ParentLocationId { get; set; }
    public bool IsActive { get; set; } = true;

    public Location? ParentLocation { get; set; }
    public ICollection<Location> ChildLocations { get; set; } = [];
    public ICollection<Asset> Assets { get; set; } = [];
    public ICollection<AssetMovement> Movements { get; set; } = [];
}

public class AssetMovement : AuditableEntity
{
    public Guid AssetId { get; set; }
    public Guid PersonId { get; set; }
    public Guid LocationId { get; set; }
    public MovementType MovementType { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public string? ValidationMethod { get; set; }

    public Asset Asset { get; set; } = null!;
    public Person Person { get; set; } = null!;
    public Location Location { get; set; } = null!;
}

public class Alert : AuditableEntity
{
    public Guid? AssetId { get; set; }
    public AlertType AlertType { get; set; }
    public AlertSeverity Severity { get; set; }
    public AlertStatus Status { get; set; } = AlertStatus.Active;
    public string Message { get; set; } = string.Empty;
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedBy { get; set; }

    public Asset? Asset { get; set; }
}

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public AuditAction Action { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool Result { get; set; } = true;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}

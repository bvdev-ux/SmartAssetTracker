using AssetTracking.Domain.Common;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Domain.Entities;

public class Person : AuditableEntity
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string? UniversityCode { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Career { get; set; }
    public string? Faculty { get; set; }
    public string? CardQrCode { get; set; }
    public PersonType PersonType { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<AssetMovement> Movements { get; set; } = [];
    public ICollection<Asset> OwnedAssets { get; set; } = [];
}

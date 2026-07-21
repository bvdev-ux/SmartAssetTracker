namespace AssetTracking.Application.Common;

public record AuditContext(Guid? UserId, string? IpAddress, string? UserAgent)
{
    public static readonly AuditContext Empty = new(null, null, null);
}

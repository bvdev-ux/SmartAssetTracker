using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.DTOs.Audit;

public record AuditLogDto(
    Guid Id,
    Guid? UserId,
    string? UserEmail,
    AuditAction Action,
    string EntityName,
    Guid? EntityId,
    string? Details,
    string? IpAddress,
    string? UserAgent,
    bool Result,
    DateTime OccurredAt);

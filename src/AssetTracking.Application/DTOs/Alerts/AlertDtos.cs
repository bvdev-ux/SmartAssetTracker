using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.DTOs.Alerts;

public record AlertDto(
    Guid Id,
    Guid? AssetId,
    string? AssetTag,
    AlertType AlertType,
    AlertSeverity Severity,
    AlertStatus Status,
    string Message,
    DateTime? ResolvedAt,
    Guid? ResolvedBy,
    DateTime CreatedAt);

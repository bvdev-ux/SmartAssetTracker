using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.DTOs.Movements;

public record MovementDto(
    Guid Id,
    Guid AssetId,
    string AssetTag,
    string CategoryName,
    Guid PersonId,
    string PersonFullName,
    Guid LocationId,
    string LocationName,
    MovementType MovementType,
    DateTime OccurredAt,
    string? Notes,
    string? ValidationMethod);

public record RegisterMovementRequest(
    string AssetIdentifier,
    string PersonIdentifier,
    Guid LocationId,
    MovementType MovementType,
    string? Notes = null,
    string? ValidationMethod = null);

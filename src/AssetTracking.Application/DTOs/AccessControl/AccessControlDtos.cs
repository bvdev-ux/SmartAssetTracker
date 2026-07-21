using AssetTracking.Application.DTOs.Movements;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.DTOs.AccessControl;

public record ValidateAccessRequest(
    string PersonIdentifier,
    string AssetIdentifier,
    Guid LocationId,
    MovementType MovementType,
    string ValidationMethod);

public record AccessValidationResult(
    bool Authorized,
    string? DenialReason,
    string? DenialCode,
    MovementDto? Movement);

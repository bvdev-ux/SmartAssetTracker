using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.DTOs.Locations;

public record LocationDto(
    Guid Id,
    string Name,
    string Code,
    LocationType LocationType,
    Guid? ParentLocationId,
    string? ParentLocationName,
    bool IsActive);

public record LocationTreeDto(
    Guid Id,
    string Name,
    string Code,
    LocationType LocationType,
    bool IsActive,
    IReadOnlyList<LocationTreeDto> Children);

public record CreateLocationRequest(
    string Name,
    string Code,
    LocationType LocationType,
    Guid? ParentLocationId);

public record UpdateLocationRequest(
    string Name,
    string Code,
    LocationType LocationType,
    Guid? ParentLocationId,
    bool IsActive);

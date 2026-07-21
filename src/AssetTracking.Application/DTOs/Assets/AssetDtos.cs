using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.DTOs.Assets;

public record AssetDto(
    Guid Id,
    string AssetTag,
    string? SerialNumber,
    string? QrCode,
    string? RfidTag,
    Guid CategoryId,
    string CategoryName,
    Guid ModelId,
    string ModelName,
    string BrandName,
    string? Color,
    string? Features,
    string? PhotoUrl,
    Guid? OwnerId,
    string? OwnerFullName,
    AssetStatusType Status,
    Guid? CurrentLocationId,
    string? CurrentLocationName,
    bool IsInsideCampus,
    string? Notes);

public record CreateAssetRequest(
    string AssetTag,
    string? SerialNumber,
    string? RfidTag,
    Guid CategoryId,
    Guid ModelId,
    string? Color,
    string? Features,
    string? PhotoUrl,
    Guid? OwnerId,
    Guid? CurrentLocationId,
    string? Notes);

public record UpdateAssetRequest(
    string AssetTag,
    string? SerialNumber,
    string? RfidTag,
    Guid CategoryId,
    Guid ModelId,
    string? Color,
    string? Features,
    string? PhotoUrl,
    Guid? OwnerId,
    AssetStatusType Status,
    string? Notes);

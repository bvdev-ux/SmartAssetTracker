namespace AssetTracking.Application.DTOs.Assets;

public record AssetCategoryDto(Guid Id, string Name, string? Description, bool IsActive);
public record CreateAssetCategoryRequest(string Name, string? Description);
public record UpdateAssetCategoryRequest(string Name, string? Description, bool IsActive);

public record AssetBrandDto(Guid Id, string Name, bool IsActive);
public record CreateAssetBrandRequest(string Name);
public record UpdateAssetBrandRequest(string Name, bool IsActive);

public record AssetModelDto(Guid Id, string Name, Guid BrandId, string BrandName, bool IsActive);
public record CreateAssetModelRequest(string Name, Guid BrandId);
public record UpdateAssetModelRequest(string Name, Guid BrandId, bool IsActive);

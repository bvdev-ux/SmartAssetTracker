using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Assets;
using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Domain.Interfaces;

namespace AssetTracking.Application.Services;

public interface IAssetService
{
    Task<PagedResult<AssetDto>> GetPagedAsync(
        string? search, Guid? categoryId, AssetStatusType? status, bool? insideCampus, int page, int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<AssetDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<AssetDto>> CreateAsync(
        CreateAssetRequest request, AuditContext audit, CancellationToken cancellationToken = default);

    Task<Result<AssetDto>> UpdateAsync(
        Guid id, UpdateAssetRequest request, AuditContext audit, CancellationToken cancellationToken = default);

    Task<Result> RetireAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default);
}

public class AssetService(
    IAssetRepository assetRepository,
    IRepository<AssetCategory> categoryRepository,
    IRepository<AssetModel> modelRepository,
    IUnitOfWork unitOfWork,
    IAuditService auditService) : IAssetService
{
    public async Task<PagedResult<AssetDto>> GetPagedAsync(
        string? search, Guid? categoryId, AssetStatusType? status, bool? insideCampus, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await assetRepository.SearchAsync(
            search, categoryId, status, insideCampus, page, pageSize, cancellationToken);

        return new PagedResult<AssetDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Result<AssetDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return asset is null
            ? Result<AssetDto>.Failure("Activo no encontrado.", "ASSET_NOT_FOUND")
            : Result<AssetDto>.Success(ToDto(asset));
    }

    public async Task<Result<AssetDto>> CreateAsync(
        CreateAssetRequest request, AuditContext audit, CancellationToken cancellationToken = default)
    {
        if (await assetRepository.ExistsByAssetTagAsync(request.AssetTag, null, cancellationToken))
        {
            return Result<AssetDto>.Failure("Ya existe un activo con ese código.", "ASSET_DUPLICATE_TAG");
        }

        if (!string.IsNullOrWhiteSpace(request.SerialNumber) &&
            await assetRepository.ExistsBySerialNumberAsync(request.SerialNumber, null, cancellationToken))
        {
            return Result<AssetDto>.Failure("Ya existe un activo con ese número de serie.", "ASSET_DUPLICATE_SERIAL");
        }

        if (await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken) is null)
        {
            return Result<AssetDto>.Failure("Categoría no encontrada.", "CATEGORY_NOT_FOUND");
        }

        if (await modelRepository.GetByIdAsync(request.ModelId, cancellationToken) is null)
        {
            return Result<AssetDto>.Failure("Modelo no encontrado.", "MODEL_NOT_FOUND");
        }

        var asset = new Asset
        {
            AssetTag = request.AssetTag,
            SerialNumber = request.SerialNumber,
            QrCode = GenerateQrCode(),
            RfidTag = request.RfidTag,
            CategoryId = request.CategoryId,
            ModelId = request.ModelId,
            Color = request.Color,
            Features = request.Features,
            PhotoUrl = request.PhotoUrl,
            OwnerId = request.OwnerId,
            CurrentLocationId = request.CurrentLocationId,
            Notes = request.Notes
        };

        await assetRepository.AddAsync(asset, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Create, nameof(Asset), asset.Id,
            $"Registró el activo {asset.AssetTag}", audit.IpAddress, audit.UserAgent,
            cancellationToken: cancellationToken);

        var created = await assetRepository.GetByIdWithDetailsAsync(asset.Id, cancellationToken);
        return Result<AssetDto>.Success(ToDto(created!));
    }

    public async Task<Result<AssetDto>> UpdateAsync(
        Guid id, UpdateAssetRequest request, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null) return Result<AssetDto>.Failure("Activo no encontrado.", "ASSET_NOT_FOUND");

        if (await assetRepository.ExistsByAssetTagAsync(request.AssetTag, id, cancellationToken))
        {
            return Result<AssetDto>.Failure("Ya existe un activo con ese código.", "ASSET_DUPLICATE_TAG");
        }

        asset.AssetTag = request.AssetTag;
        asset.SerialNumber = request.SerialNumber;
        asset.RfidTag = request.RfidTag;
        asset.CategoryId = request.CategoryId;
        asset.ModelId = request.ModelId;
        asset.Color = request.Color;
        asset.Features = request.Features;
        asset.PhotoUrl = request.PhotoUrl;
        asset.OwnerId = request.OwnerId;
        asset.Status = request.Status;
        asset.Notes = request.Notes;

        assetRepository.Update(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Update, nameof(Asset), asset.Id,
            ipAddress: audit.IpAddress, userAgent: audit.UserAgent, cancellationToken: cancellationToken);

        var updated = await assetRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return Result<AssetDto>.Success(ToDto(updated!));
    }

    public async Task<Result> RetireAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null) return Result.Failure("Activo no encontrado.", "ASSET_NOT_FOUND");

        asset.Status = AssetStatusType.Retired;
        assetRepository.Update(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Delete, nameof(Asset), asset.Id,
            "Dio de baja el activo", audit.IpAddress, audit.UserAgent, cancellationToken: cancellationToken);

        return Result.Success();
    }

    private static string GenerateQrCode() => $"AST-{Guid.NewGuid():N}"[..16].ToUpperInvariant();

    private static AssetDto ToDto(Asset a) => new(
        a.Id, a.AssetTag, a.SerialNumber, a.QrCode, a.RfidTag,
        a.CategoryId, a.Category.Name, a.ModelId, a.Model.Name, a.Model.Brand.Name,
        a.Color, a.Features, a.PhotoUrl,
        a.OwnerId, a.Owner is null ? null : $"{a.Owner.FirstName} {a.Owner.LastName}",
        a.Status, a.CurrentLocationId, a.CurrentLocation?.Name,
        a.IsInsideCampus, a.Notes);
}

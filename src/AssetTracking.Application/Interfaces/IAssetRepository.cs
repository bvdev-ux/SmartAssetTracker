using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.Interfaces;

public interface IAssetRepository : Domain.Interfaces.IRepository<Asset>
{
    Task<Asset?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Asset?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Asset> Items, int TotalCount)> SearchAsync(
        string? search,
        Guid? categoryId,
        AssetStatusType? status,
        bool? insideCampus,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Asset>> GetRetainedAsync(TimeSpan threshold, CancellationToken cancellationToken = default);

    Task<bool> ExistsByAssetTagAsync(
        string assetTag,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsBySerialNumberAsync(
        string serialNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}

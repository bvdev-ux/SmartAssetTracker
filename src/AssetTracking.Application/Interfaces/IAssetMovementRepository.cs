using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.Interfaces;

public record AssetUsageCount(Guid AssetId, string AssetTag, string CategoryName, int MovementCount);

public interface IAssetMovementRepository : Domain.Interfaces.IRepository<AssetMovement>
{
    Task<AssetMovement?> GetLastByAssetAsync(Guid assetId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AssetMovement> Items, int TotalCount)> GetHistoryByAssetAsync(
        Guid assetId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AssetMovement> Items, int TotalCount)> GetHistoryByPersonAsync(
        Guid personId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AssetMovement>> GetByDateRangeAsync(
        DateTime fromUtc,
        DateTime toUtc,
        MovementType? movementType = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AssetUsageCount>> GetMostUsedAssetsAsync(
        int top,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default);
}

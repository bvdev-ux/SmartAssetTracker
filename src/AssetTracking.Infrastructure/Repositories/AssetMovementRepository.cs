using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Infrastructure.Repositories;

public class AssetMovementRepository(ApplicationDbContext context)
    : Repository<AssetMovement>(context), IAssetMovementRepository
{
    private static IQueryable<AssetMovement> WithDetails(IQueryable<AssetMovement> query) =>
        query.Include(m => m.Asset).ThenInclude(a => a.Category)
            .Include(m => m.Person)
            .Include(m => m.Location);

    public async Task<AssetMovement?> GetLastByAssetAsync(
        Guid assetId,
        CancellationToken cancellationToken = default) =>
        await Context.AssetMovements
            .AsNoTracking()
            .Where(m => m.AssetId == assetId)
            .OrderByDescending(m => m.OccurredAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<(IReadOnlyList<AssetMovement> Items, int TotalCount)> GetHistoryByAssetAsync(
        Guid assetId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = WithDetails(Context.AssetMovements.AsNoTracking())
            .Where(m => m.AssetId == assetId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(m => m.OccurredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<AssetMovement> Items, int TotalCount)> GetHistoryByPersonAsync(
        Guid personId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = WithDetails(Context.AssetMovements.AsNoTracking())
            .Where(m => m.PersonId == personId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(m => m.OccurredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<AssetMovement>> GetByDateRangeAsync(
        DateTime fromUtc,
        DateTime toUtc,
        MovementType? movementType = null,
        CancellationToken cancellationToken = default)
    {
        var query = WithDetails(Context.AssetMovements.AsNoTracking())
            .Where(m => m.OccurredAt >= fromUtc && m.OccurredAt < toUtc);

        if (movementType.HasValue) query = query.Where(m => m.MovementType == movementType.Value);

        return await query.OrderByDescending(m => m.OccurredAt).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AssetUsageCount>> GetMostUsedAssetsAsync(
        int top,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.AssetMovements.AsNoTracking().AsQueryable();

        if (fromUtc.HasValue) query = query.Where(m => m.OccurredAt >= fromUtc.Value);
        if (toUtc.HasValue) query = query.Where(m => m.OccurredAt < toUtc.Value);

        return await query
            .GroupBy(m => new { m.AssetId, m.Asset.AssetTag, CategoryName = m.Asset.Category.Name })
            .Select(g => new AssetUsageCount(g.Key.AssetId, g.Key.AssetTag, g.Key.CategoryName, g.Count()))
            .OrderByDescending(u => u.MovementCount)
            .Take(top)
            .ToListAsync(cancellationToken);
    }
}

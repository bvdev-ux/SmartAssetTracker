using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Infrastructure.Repositories;

public class AssetRepository(ApplicationDbContext context) : Repository<Asset>(context), IAssetRepository
{
    private static IQueryable<Asset> WithDetails(IQueryable<Asset> query) =>
        query
            .Include(a => a.Category)
            .Include(a => a.Model).ThenInclude(m => m.Brand)
            .Include(a => a.Owner)
            .Include(a => a.CurrentLocation);

    public async Task<Asset?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await WithDetails(Context.Assets.AsNoTracking())
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Asset?> GetByIdentifierAsync(
        string identifier,
        CancellationToken cancellationToken = default) =>
        await WithDetails(Context.Assets.AsQueryable())
            .FirstOrDefaultAsync(
                a => a.AssetTag == identifier || a.QrCode == identifier || a.SerialNumber == identifier,
                cancellationToken);

    public async Task<(IReadOnlyList<Asset> Items, int TotalCount)> SearchAsync(
        string? search,
        Guid? categoryId,
        AssetStatusType? status,
        bool? insideCampus,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = WithDetails(Context.Assets.AsNoTracking());

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(a =>
                a.AssetTag.ToLower().Contains(term) ||
                (a.SerialNumber != null && a.SerialNumber.ToLower().Contains(term)) ||
                a.Model.Name.ToLower().Contains(term) ||
                a.Model.Brand.Name.ToLower().Contains(term));
        }

        if (categoryId.HasValue) query = query.Where(a => a.CategoryId == categoryId.Value);
        if (status.HasValue) query = query.Where(a => a.Status == status.Value);
        if (insideCampus.HasValue) query = query.Where(a => a.IsInsideCampus == insideCampus.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Asset>> GetRetainedAsync(
        TimeSpan threshold,
        CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow - threshold;
        return await WithDetails(Context.Assets.AsNoTracking())
            .Where(a => !a.IsInsideCampus && (a.UpdatedAt ?? a.CreatedAt) <= cutoff)
            .OrderBy(a => a.UpdatedAt ?? a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByAssetTagAsync(
        string assetTag,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default) =>
        await Context.Assets.AnyAsync(
            a => a.AssetTag == assetTag && (excludeId == null || a.Id != excludeId),
            cancellationToken);

    public async Task<bool> ExistsBySerialNumberAsync(
        string serialNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default) =>
        await Context.Assets.AnyAsync(
            a => a.SerialNumber == serialNumber && (excludeId == null || a.Id != excludeId),
            cancellationToken);
}

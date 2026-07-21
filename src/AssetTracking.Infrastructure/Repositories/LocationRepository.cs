using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Infrastructure.Repositories;

public class LocationRepository(ApplicationDbContext context) : Repository<Location>(context), ILocationRepository
{
    public async Task<IReadOnlyList<Location>> GetAllWithHierarchyAsync(
        CancellationToken cancellationToken = default) =>
        await Context.Locations
            .AsNoTracking()
            .Include(l => l.ChildLocations)
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default) =>
        await Context.Locations.AnyAsync(
            l => l.Code == code && (excludeId == null || l.Id != excludeId),
            cancellationToken);
}

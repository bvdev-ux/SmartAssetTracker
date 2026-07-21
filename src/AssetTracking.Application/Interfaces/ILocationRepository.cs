using AssetTracking.Domain.Entities;

namespace AssetTracking.Application.Interfaces;

public interface ILocationRepository : Domain.Interfaces.IRepository<Location>
{
    Task<IReadOnlyList<Location>> GetAllWithHierarchyAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}

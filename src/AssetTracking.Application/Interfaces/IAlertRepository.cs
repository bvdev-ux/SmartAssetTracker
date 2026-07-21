using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.Interfaces;

public interface IAlertRepository : Domain.Interfaces.IRepository<Alert>
{
    Task<Alert?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Alert> Items, int TotalCount)> SearchAsync(
        AlertStatus? status,
        AlertSeverity? severity,
        AlertType? alertType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Infrastructure.Repositories;

public class AlertRepository(ApplicationDbContext context) : Repository<Alert>(context), IAlertRepository
{
    public async Task<Alert?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await Context.Alerts
            .AsNoTracking()
            .Include(a => a.Asset)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Alert> Items, int TotalCount)> SearchAsync(
        AlertStatus? status,
        AlertSeverity? severity,
        AlertType? alertType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Alerts.AsNoTracking().Include(a => a.Asset).AsQueryable();

        if (status.HasValue) query = query.Where(a => a.Status == status.Value);
        if (severity.HasValue) query = query.Where(a => a.Severity == severity.Value);
        if (alertType.HasValue) query = query.Where(a => a.AlertType == alertType.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}

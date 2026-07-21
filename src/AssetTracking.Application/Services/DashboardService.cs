using AssetTracking.Application.DTOs.Dashboard;
using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}

public class DashboardService(
    IAssetRepository assetRepository,
    IAssetMovementRepository movementRepository,
    IAlertRepository alertRepository) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        var assetsInside = await assetRepository.CountAsync(a => a.IsInsideCampus, cancellationToken);
        var assetsOutside = await assetRepository.CountAsync(a => !a.IsInsideCampus, cancellationToken);

        var todaysMovements = await movementRepository.GetByDateRangeAsync(todayStart, todayEnd, cancellationToken: cancellationToken);

        var entriesToday = todaysMovements.Count(m =>
            m.MovementType == MovementType.Entry || m.MovementType == MovementType.ReEntry);
        var exitsToday = todaysMovements.Count(m => m.MovementType == MovementType.Exit);
        var visitorsToday = todaysMovements
            .Where(m => m.Person.PersonType == PersonType.Visitor)
            .Select(m => m.PersonId)
            .Distinct()
            .Count();

        var activeAlerts = await alertRepository.CountAsync(a => a.Status == AlertStatus.Active, cancellationToken);

        return new DashboardSummaryDto(assetsInside, assetsOutside, entriesToday, exitsToday, activeAlerts, visitorsToday);
    }
}

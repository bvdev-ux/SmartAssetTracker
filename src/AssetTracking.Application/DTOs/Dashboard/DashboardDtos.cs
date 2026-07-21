namespace AssetTracking.Application.DTOs.Dashboard;

public record DashboardSummaryDto(
    int AssetsInside,
    int AssetsOutside,
    int EntriesToday,
    int ExitsToday,
    int ActiveAlerts,
    int VisitorsToday);

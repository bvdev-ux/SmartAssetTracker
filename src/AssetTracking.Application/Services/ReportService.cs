using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Alerts;
using AssetTracking.Application.DTOs.Assets;
using AssetTracking.Application.DTOs.Movements;
using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.Services;

public interface IReportService
{
    Task<IReadOnlyList<MovementDto>> GetEntriesOfDayAsync(DateOnly date, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MovementDto>> GetExitsOfDayAsync(DateOnly date, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AssetUsageCount>> GetMostUsedAssetsAsync(
        int top, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AssetDto>> GetRetainedAssetsAsync(int hoursThreshold, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<MovementDto>>> GetAssetHistoryAsync(Guid assetId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<MovementDto>>> GetPersonHistoryAsync(Guid personId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AlertDto>> GetAlertsAsync(
        AlertStatus? status, AlertSeverity? severity, AlertType? alertType, CancellationToken cancellationToken = default);

    byte[] ExportMovements(string title, IReadOnlyList<MovementDto> movements, ExportFormat format);

    byte[] ExportAssetUsage(string title, IReadOnlyList<AssetUsageCount> usage, ExportFormat format);

    byte[] ExportAssets(string title, IReadOnlyList<AssetDto> assets, ExportFormat format);

    byte[] ExportAlerts(string title, IReadOnlyList<AlertDto> alerts, ExportFormat format);
}

public class ReportService(
    IAssetMovementRepository movementRepository,
    IAssetRepository assetRepository,
    IAlertRepository alertRepository,
    IPersonRepository personRepository,
    IReportExportService exportService) : IReportService
{
    public async Task<IReadOnlyList<MovementDto>> GetEntriesOfDayAsync(
        DateOnly date, CancellationToken cancellationToken = default) =>
        await GetMovementsOfDayAsync(date, isEntry: true, cancellationToken);

    public async Task<IReadOnlyList<MovementDto>> GetExitsOfDayAsync(
        DateOnly date, CancellationToken cancellationToken = default) =>
        await GetMovementsOfDayAsync(date, isEntry: false, cancellationToken);

    private async Task<IReadOnlyList<MovementDto>> GetMovementsOfDayAsync(
        DateOnly date, bool isEntry, CancellationToken cancellationToken)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);

        var movements = await movementRepository.GetByDateRangeAsync(dayStart, dayEnd, cancellationToken: cancellationToken);

        return movements
            .Where(m => isEntry
                ? m.MovementType is MovementType.Entry or MovementType.ReEntry
                : m.MovementType == MovementType.Exit)
            .Select(ToDto)
            .ToList();
    }

    public async Task<IReadOnlyList<AssetUsageCount>> GetMostUsedAssetsAsync(
        int top, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default) =>
        await movementRepository.GetMostUsedAssetsAsync(
            top,
            from?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            to?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(1),
            cancellationToken);

    public async Task<IReadOnlyList<AssetDto>> GetRetainedAssetsAsync(
        int hoursThreshold, CancellationToken cancellationToken = default)
    {
        var assets = await assetRepository.GetRetainedAsync(TimeSpan.FromHours(hoursThreshold), cancellationToken);
        return assets.Select(a => new AssetDto(
            a.Id, a.AssetTag, a.SerialNumber, a.QrCode, a.RfidTag,
            a.CategoryId, a.Category.Name, a.ModelId, a.Model.Name, a.Model.Brand.Name,
            a.Color, a.Features, a.PhotoUrl,
            a.OwnerId, a.Owner is null ? null : $"{a.Owner.FirstName} {a.Owner.LastName}",
            a.Status, a.CurrentLocationId, a.CurrentLocation?.Name,
            a.IsInsideCampus, a.Notes)).ToList();
    }

    public async Task<Result<IReadOnlyList<MovementDto>>> GetAssetHistoryAsync(
        Guid assetId, CancellationToken cancellationToken = default)
    {
        if (await assetRepository.GetByIdAsync(assetId, cancellationToken) is null)
        {
            return Result<IReadOnlyList<MovementDto>>.Failure("Activo no encontrado.", "ASSET_NOT_FOUND");
        }

        var (items, _) = await movementRepository.GetHistoryByAssetAsync(assetId, 1, int.MaxValue, cancellationToken);
        return Result<IReadOnlyList<MovementDto>>.Success(items.Select(ToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<MovementDto>>> GetPersonHistoryAsync(
        Guid personId, CancellationToken cancellationToken = default)
    {
        if (await personRepository.GetByIdAsync(personId, cancellationToken) is null)
        {
            return Result<IReadOnlyList<MovementDto>>.Failure("Persona no encontrada.", "PERSON_NOT_FOUND");
        }

        var (items, _) = await movementRepository.GetHistoryByPersonAsync(personId, 1, int.MaxValue, cancellationToken);
        return Result<IReadOnlyList<MovementDto>>.Success(items.Select(ToDto).ToList());
    }

    public async Task<IReadOnlyList<AlertDto>> GetAlertsAsync(
        AlertStatus? status, AlertSeverity? severity, AlertType? alertType, CancellationToken cancellationToken = default)
    {
        var (items, _) = await alertRepository.SearchAsync(status, severity, alertType, 1, int.MaxValue, cancellationToken);
        return items.Select(a => new AlertDto(
            a.Id, a.AssetId, a.Asset?.AssetTag, a.AlertType, a.Severity, a.Status,
            a.Message, a.ResolvedAt, a.ResolvedBy, a.CreatedAt)).ToList();
    }

    public byte[] ExportMovements(string title, IReadOnlyList<MovementDto> movements, ExportFormat format)
    {
        string[] headers = ["Fecha", "Tipo", "Activo", "Categoría", "Persona", "Ubicación", "Notas"];
        var rows = movements.Select(m => (IReadOnlyList<string>)new List<string>
        {
            m.OccurredAt.ToString("yyyy-MM-dd HH:mm"),
            m.MovementType.ToString(),
            m.AssetTag,
            m.CategoryName,
            m.PersonFullName,
            m.LocationName,
            m.Notes ?? string.Empty
        }).ToList();

        return Export(title, headers, rows, format);
    }

    public byte[] ExportAssetUsage(string title, IReadOnlyList<AssetUsageCount> usage, ExportFormat format)
    {
        string[] headers = ["Código de activo", "Categoría", "Movimientos"];
        var rows = usage.Select(u => (IReadOnlyList<string>)new List<string>
        {
            u.AssetTag, u.CategoryName, u.MovementCount.ToString()
        }).ToList();

        return Export(title, headers, rows, format);
    }

    public byte[] ExportAssets(string title, IReadOnlyList<AssetDto> assets, ExportFormat format)
    {
        string[] headers = ["Código", "Serie", "Categoría", "Marca", "Modelo", "Estado", "Ubicación actual", "Dentro del campus"];
        var rows = assets.Select(a => (IReadOnlyList<string>)new List<string>
        {
            a.AssetTag, a.SerialNumber ?? string.Empty, a.CategoryName, a.BrandName, a.ModelName,
            a.Status.ToString(), a.CurrentLocationName ?? string.Empty, a.IsInsideCampus ? "Sí" : "No"
        }).ToList();

        return Export(title, headers, rows, format);
    }

    public byte[] ExportAlerts(string title, IReadOnlyList<AlertDto> alerts, ExportFormat format)
    {
        string[] headers = ["Fecha", "Tipo", "Severidad", "Estado", "Activo", "Mensaje"];
        var rows = alerts.Select(a => (IReadOnlyList<string>)new List<string>
        {
            a.CreatedAt.ToString("yyyy-MM-dd HH:mm"), a.AlertType.ToString(), a.Severity.ToString(),
            a.Status.ToString(), a.AssetTag ?? string.Empty, a.Message
        }).ToList();

        return Export(title, headers, rows, format);
    }

    private byte[] Export(string title, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows, ExportFormat format) =>
        format switch
        {
            ExportFormat.Excel => exportService.ExportToExcel(title, headers, rows),
            ExportFormat.Pdf => exportService.ExportToPdf(title, headers, rows),
            _ => throw new InvalidOperationException("Formato de exportación no soportado para esta operación.")
        };

    private static MovementDto ToDto(Domain.Entities.AssetMovement m) => new(
        m.Id, m.AssetId, m.Asset.AssetTag, m.Asset.Category.Name,
        m.PersonId, $"{m.Person.FirstName} {m.Person.LastName}",
        m.LocationId, m.Location.Name, m.MovementType, m.OccurredAt, m.Notes, m.ValidationMethod);
}

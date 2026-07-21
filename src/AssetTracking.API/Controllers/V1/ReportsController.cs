using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Alerts;
using AssetTracking.Application.DTOs.Assets;
using AssetTracking.Application.DTOs.Movements;
using AssetTracking.Application.Interfaces;
using AssetTracking.Application.Services;
using AssetTracking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("entradas-del-dia")]
    public async Task<IActionResult> GetEntriesOfDay(
        [FromQuery] DateOnly? date, [FromQuery] ExportFormat format = ExportFormat.Json, CancellationToken cancellationToken = default)
    {
        var day = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var data = await reportService.GetEntriesOfDayAsync(day, cancellationToken);
        return RespondMovements($"Entradas del {day:yyyy-MM-dd}", data, format);
    }

    [HttpGet("salidas-del-dia")]
    public async Task<IActionResult> GetExitsOfDay(
        [FromQuery] DateOnly? date, [FromQuery] ExportFormat format = ExportFormat.Json, CancellationToken cancellationToken = default)
    {
        var day = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var data = await reportService.GetExitsOfDayAsync(day, cancellationToken);
        return RespondMovements($"Salidas del {day:yyyy-MM-dd}", data, format);
    }

    [HttpGet("equipos-mas-usados")]
    public async Task<IActionResult> GetMostUsedAssets(
        [FromQuery] int top = 10, [FromQuery] DateOnly? from = null, [FromQuery] DateOnly? to = null,
        [FromQuery] ExportFormat format = ExportFormat.Json, CancellationToken cancellationToken = default)
    {
        var data = await reportService.GetMostUsedAssetsAsync(top, from, to, cancellationToken);

        if (format == ExportFormat.Json) return Ok(ApiResponse<IReadOnlyList<AssetUsageCount>>.Ok(data));
        var bytes = reportService.ExportAssetUsage("Equipos más usados", data, format);
        return AsFile(bytes, format, "equipos-mas-usados");
    }

    [HttpGet("equipos-retenidos")]
    public async Task<IActionResult> GetRetainedAssets(
        [FromQuery] int hoursThreshold = 24, [FromQuery] ExportFormat format = ExportFormat.Json, CancellationToken cancellationToken = default)
    {
        var data = await reportService.GetRetainedAssetsAsync(hoursThreshold, cancellationToken);

        if (format == ExportFormat.Json) return Ok(ApiResponse<IReadOnlyList<AssetDto>>.Ok(data));
        var bytes = reportService.ExportAssets("Equipos retenidos", data, format);
        return AsFile(bytes, format, "equipos-retenidos");
    }

    [HttpGet("historial-activo/{assetId:guid}")]
    public async Task<IActionResult> GetAssetHistory(
        Guid assetId, [FromQuery] ExportFormat format = ExportFormat.Json, CancellationToken cancellationToken = default)
    {
        var result = await reportService.GetAssetHistoryAsync(assetId, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return RespondMovements($"Historial del activo {assetId}", result.Value!, format);
    }

    [HttpGet("historial-persona/{personId:guid}")]
    public async Task<IActionResult> GetPersonHistory(
        Guid personId, [FromQuery] ExportFormat format = ExportFormat.Json, CancellationToken cancellationToken = default)
    {
        var result = await reportService.GetPersonHistoryAsync(personId, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return RespondMovements($"Historial de la persona {personId}", result.Value!, format);
    }

    [HttpGet("alertas")]
    public async Task<IActionResult> GetAlerts(
        [FromQuery] AlertStatus? status, [FromQuery] AlertSeverity? severity, [FromQuery] AlertType? alertType,
        [FromQuery] ExportFormat format = ExportFormat.Json, CancellationToken cancellationToken = default)
    {
        var data = await reportService.GetAlertsAsync(status, severity, alertType, cancellationToken);

        if (format == ExportFormat.Json) return Ok(ApiResponse<IReadOnlyList<AlertDto>>.Ok(data));
        var bytes = reportService.ExportAlerts("Reporte de alertas", data, format);
        return AsFile(bytes, format, "alertas");
    }

    private IActionResult RespondMovements(string title, IReadOnlyList<MovementDto> data, ExportFormat format)
    {
        if (format == ExportFormat.Json) return Ok(ApiResponse<IReadOnlyList<MovementDto>>.Ok(data));
        var bytes = reportService.ExportMovements(title, data, format);
        return AsFile(bytes, format, title.ToLowerInvariant().Replace(' ', '-'));
    }

    private static FileContentResult AsFile(byte[] bytes, ExportFormat format, string fileNameWithoutExtension) =>
        format == ExportFormat.Excel
            ? new FileContentResult(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                { FileDownloadName = $"{fileNameWithoutExtension}.xlsx" }
            : new FileContentResult(bytes, "application/pdf")
                { FileDownloadName = $"{fileNameWithoutExtension}.pdf" };
}

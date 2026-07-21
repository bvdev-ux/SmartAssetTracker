using AssetTracking.API.Extensions;
using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Alerts;
using AssetTracking.Application.Services;
using AssetTracking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AlertsController(IAlertService alertService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AlertDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] AlertStatus? status,
        [FromQuery] AlertSeverity? severity,
        [FromQuery] AlertType? alertType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await alertService.GetPagedAsync(status, severity, alertType, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedResult<AlertDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AlertDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await alertService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<AlertDto>.Ok(result.Value!));
    }

    [HttpPost("{id:guid}/resolver")]
    [ProducesResponseType(typeof(ApiResponse<AlertDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resolve(Guid id, CancellationToken cancellationToken)
    {
        var result = await alertService.ResolveAsync(id, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<AlertDto>.Ok(result.Value!, "Alerta resuelta."));
    }

    [HttpPost("{id:guid}/reconocer")]
    [ProducesResponseType(typeof(ApiResponse<AlertDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Acknowledge(Guid id, CancellationToken cancellationToken)
    {
        var result = await alertService.AcknowledgeAsync(id, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<AlertDto>.Ok(result.Value!, "Alerta reconocida."));
    }

    [HttpPost("escanear-permanencia")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ScanExceededPermanence(
        [FromQuery] int hoursThreshold = 24, CancellationToken cancellationToken = default)
    {
        var created = await alertService.ScanExceededPermanenceAsync(TimeSpan.FromHours(hoursThreshold), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { AlertsCreated = created }, "Escaneo de permanencia completado."));
    }
}

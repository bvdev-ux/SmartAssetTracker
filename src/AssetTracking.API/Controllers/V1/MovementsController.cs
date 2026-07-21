using AssetTracking.API.Extensions;
using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Movements;
using AssetTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MovementsController(IMovementService movementService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MovementDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterMovementRequest request, CancellationToken cancellationToken)
    {
        var result = await movementService.RegisterAsync(request, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return BadRequest(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<MovementDto>.Ok(result.Value!, "Movimiento registrado."));
    }

    [HttpGet("activo/{assetId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MovementDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistoryByAsset(
        Guid assetId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await movementService.GetHistoryByAssetAsync(assetId, page, pageSize, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<PagedResult<MovementDto>>.Ok(result.Value!));
    }
}

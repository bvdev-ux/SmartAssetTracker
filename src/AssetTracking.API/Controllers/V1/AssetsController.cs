using AssetTracking.API.Extensions;
using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Assets;
using AssetTracking.Application.Services;
using AssetTracking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AssetsController(IAssetService assetService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AssetDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] AssetStatusType? status,
        [FromQuery] bool? insideCampus,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await assetService.GetPagedAsync(search, categoryId, status, insideCampus, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedResult<AssetDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await assetService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<AssetDto>.Ok(result.Value!));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AssetDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAssetRequest request, CancellationToken cancellationToken)
    {
        var result = await assetService.CreateAsync(request, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return BadRequest(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<AssetDto>.Ok(result.Value!, "Activo registrado."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssetRequest request, CancellationToken cancellationToken)
    {
        var result = await assetService.UpdateAsync(id, request, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return BadRequest(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<AssetDto>.Ok(result.Value!, "Activo actualizado."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Retire(Guid id, CancellationToken cancellationToken)
    {
        var result = await assetService.RetireAsync(id, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<object>.Ok(new { }, "Activo dado de baja."));
    }
}

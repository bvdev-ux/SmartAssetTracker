using AssetTracking.API.Extensions;
using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Locations;
using AssetTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LocationsController(ILocationService locationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LocationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await locationService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<LocationDto>>.Ok(result));
    }

    [HttpGet("arbol")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LocationTreeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTree(CancellationToken cancellationToken)
    {
        var result = await locationService.GetTreeAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<LocationTreeDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await locationService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<LocationDto>.Ok(result.Value!));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await locationService.CreateAsync(request, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return BadRequest(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<LocationDto>.Ok(result.Value!, "Ubicación registrada."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await locationService.UpdateAsync(id, request, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return BadRequest(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<LocationDto>.Ok(result.Value!, "Ubicación actualizada."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await locationService.DeactivateAsync(id, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<object>.Ok(new { }, "Ubicación desactivada."));
    }
}

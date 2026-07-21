using AssetTracking.API.Extensions;
using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Movements;
using AssetTracking.Application.DTOs.People;
using AssetTracking.Application.Services;
using AssetTracking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PeopleController(IPersonService personService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PersonDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] PersonType? personType,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await personService.GetPagedAsync(search, personType, isActive, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedResult<PersonDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await personService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<PersonDto>.Ok(result.Value!));
    }

    [HttpGet("{id:guid}/movimientos")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MovementDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovementHistory(
        Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await personService.GetMovementHistoryAsync(id, page, pageSize, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<PagedResult<MovementDto>>.Ok(result.Value!));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PersonDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request, CancellationToken cancellationToken)
    {
        var result = await personService.CreateAsync(request, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return BadRequest(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<PersonDto>.Ok(result.Value!, "Persona registrada."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonRequest request, CancellationToken cancellationToken)
    {
        var result = await personService.UpdateAsync(id, request, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<PersonDto>.Ok(result.Value!, "Persona actualizada."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await personService.DeactivateAsync(id, this.GetAuditContext(), cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<object>.Ok(new { }, "Persona desactivada."));
    }
}

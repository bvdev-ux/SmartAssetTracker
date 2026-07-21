using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Assets;
using AssetTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AssetModelsController(IAssetModelService modelService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? brandId, CancellationToken cancellationToken)
    {
        var result = await modelService.GetAllAsync(brandId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AssetModelDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssetModelRequest request, CancellationToken cancellationToken)
    {
        var result = await modelService.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess) return BadRequest(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<AssetModelDto>.Ok(result.Value!, "Modelo registrado."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssetModelRequest request, CancellationToken cancellationToken)
    {
        var result = await modelService.UpdateAsync(id, request, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<AssetModelDto>.Ok(result.Value!, "Modelo actualizado."));
    }
}

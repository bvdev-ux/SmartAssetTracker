using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Assets;
using AssetTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AssetBrandsController(IAssetBrandService brandService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await brandService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AssetBrandDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssetBrandRequest request, CancellationToken cancellationToken)
    {
        var result = await brandService.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess) return BadRequest(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<AssetBrandDto>.Ok(result.Value!, "Marca registrada."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssetBrandRequest request, CancellationToken cancellationToken)
    {
        var result = await brandService.UpdateAsync(id, request, cancellationToken);
        if (!result.IsSuccess) return NotFound(ApiResponse<object>.Fail(result.Error!, [result.ErrorCode!]));
        return Ok(ApiResponse<AssetBrandDto>.Ok(result.Value!, "Marca actualizada."));
    }
}

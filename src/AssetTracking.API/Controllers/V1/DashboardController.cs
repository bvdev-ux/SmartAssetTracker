using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Dashboard;
using AssetTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("resumen")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetSummaryAsync(cancellationToken);
        return Ok(ApiResponse<DashboardSummaryDto>.Ok(result));
    }
}

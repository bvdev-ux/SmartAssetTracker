using AssetTracking.API.Extensions;
using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.AccessControl;
using AssetTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AccessControlController(IAccessControlService accessControlService) : ControllerBase
{
    [HttpPost("validar")]
    [ProducesResponseType(typeof(ApiResponse<AccessValidationResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Validate([FromBody] ValidateAccessRequest request, CancellationToken cancellationToken)
    {
        var result = await accessControlService.ValidateAsync(request, this.GetAuditContext(), cancellationToken);

        return result.Authorized
            ? Ok(ApiResponse<AccessValidationResult>.Ok(result, "Acceso autorizado."))
            : StatusCode(StatusCodes.Status403Forbidden, ApiResponse<AccessValidationResult>.Fail(
                result.DenialReason ?? "Acceso denegado.", [result.DenialCode ?? "ACCESS_DENIED"]));
    }
}

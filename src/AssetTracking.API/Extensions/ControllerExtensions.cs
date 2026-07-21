using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AssetTracking.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracking.API.Extensions;

public static class ControllerExtensions
{
    public static AuditContext GetAuditContext(this ControllerBase controller)
    {
        var claim = controller.User.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? controller.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = claim is not null && Guid.TryParse(claim.Value, out var id) ? id : (Guid?)null;
        var ipAddress = controller.HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = controller.Request.Headers.UserAgent.ToString();

        return new AuditContext(userId, ipAddress, userAgent);
    }
}

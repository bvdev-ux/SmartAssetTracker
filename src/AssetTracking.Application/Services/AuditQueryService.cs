using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Audit;
using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.Services;

public interface IAuditQueryService
{
    Task<PagedResult<AuditLogDto>> GetPagedAsync(
        Guid? userId, string? entityName, AuditAction? action, DateTime? fromUtc, DateTime? toUtc,
        int page, int pageSize, CancellationToken cancellationToken = default);
}

public class AuditQueryService(IAuditLogRepository auditLogRepository) : IAuditQueryService
{
    public async Task<PagedResult<AuditLogDto>> GetPagedAsync(
        Guid? userId, string? entityName, AuditAction? action, DateTime? fromUtc, DateTime? toUtc,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, total) = await auditLogRepository.SearchAsync(
            userId, entityName, action, fromUtc, toUtc, page, pageSize, cancellationToken);

        return new PagedResult<AuditLogDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    private static AuditLogDto ToDto(AuditLog a) => new(
        a.Id, a.UserId, a.User?.Email, a.Action, a.EntityName, a.EntityId,
        a.Details, a.IpAddress, a.UserAgent, a.Result, a.OccurredAt);
}

using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.Interfaces;

public interface IAuditLogRepository : Domain.Interfaces.IRepository<AuditLog>
{
    Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> SearchAsync(
        Guid? userId,
        string? entityName,
        AuditAction? action,
        DateTime? fromUtc,
        DateTime? toUtc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

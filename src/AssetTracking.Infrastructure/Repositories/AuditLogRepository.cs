using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Infrastructure.Repositories;

public class AuditLogRepository(ApplicationDbContext context)
    : Repository<AuditLog>(context), IAuditLogRepository
{
    public async Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> SearchAsync(
        Guid? userId,
        string? entityName,
        AuditAction? action,
        DateTime? fromUtc,
        DateTime? toUtc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.AuditLogs.AsNoTracking().Include(a => a.User).AsQueryable();

        if (userId.HasValue) query = query.Where(a => a.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(entityName)) query = query.Where(a => a.EntityName == entityName);
        if (action.HasValue) query = query.Where(a => a.Action == action.Value);
        if (fromUtc.HasValue) query = query.Where(a => a.OccurredAt >= fromUtc.Value);
        if (toUtc.HasValue) query = query.Where(a => a.OccurredAt < toUtc.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.OccurredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}

using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Alerts;
using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Domain.Interfaces;

namespace AssetTracking.Application.Services;

public interface IAlertService
{
    Task<PagedResult<AlertDto>> GetPagedAsync(
        AlertStatus? status, AlertSeverity? severity, AlertType? alertType, int page, int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<AlertDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<AlertDto>> ResolveAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default);

    Task<Result<AlertDto>> AcknowledgeAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default);

    Task<int> ScanExceededPermanenceAsync(TimeSpan threshold, CancellationToken cancellationToken = default);
}

public class AlertService(
    IAlertRepository alertRepository,
    IAssetRepository assetRepository,
    IUnitOfWork unitOfWork,
    IAuditService auditService) : IAlertService
{
    public async Task<PagedResult<AlertDto>> GetPagedAsync(
        AlertStatus? status, AlertSeverity? severity, AlertType? alertType, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await alertRepository.SearchAsync(status, severity, alertType, page, pageSize, cancellationToken);

        return new PagedResult<AlertDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Result<AlertDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var alert = await alertRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return alert is null
            ? Result<AlertDto>.Failure("Alerta no encontrada.", "ALERT_NOT_FOUND")
            : Result<AlertDto>.Success(ToDto(alert));
    }

    public async Task<Result<AlertDto>> ResolveAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var alert = await alertRepository.GetByIdAsync(id, cancellationToken);
        if (alert is null) return Result<AlertDto>.Failure("Alerta no encontrada.", "ALERT_NOT_FOUND");

        alert.Status = AlertStatus.Resolved;
        alert.ResolvedAt = DateTime.UtcNow;
        alert.ResolvedBy = audit.UserId;
        alertRepository.Update(alert);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Update, nameof(Alert), alert.Id,
            "Resolvió la alerta", audit.IpAddress, audit.UserAgent, cancellationToken: cancellationToken);

        var withDetails = await alertRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return Result<AlertDto>.Success(ToDto(withDetails!));
    }

    public async Task<Result<AlertDto>> AcknowledgeAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var alert = await alertRepository.GetByIdAsync(id, cancellationToken);
        if (alert is null) return Result<AlertDto>.Failure("Alerta no encontrada.", "ALERT_NOT_FOUND");

        alert.Status = AlertStatus.Acknowledged;
        alertRepository.Update(alert);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Update, nameof(Alert), alert.Id,
            "Reconoció la alerta", audit.IpAddress, audit.UserAgent, cancellationToken: cancellationToken);

        var withDetails = await alertRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return Result<AlertDto>.Success(ToDto(withDetails!));
    }

    public async Task<int> ScanExceededPermanenceAsync(TimeSpan threshold, CancellationToken cancellationToken = default)
    {
        var retainedAssets = await assetRepository.GetRetainedAsync(threshold, cancellationToken);
        var created = 0;

        foreach (var asset in retainedAssets)
        {
            var hasActiveAlert = (await alertRepository.FindAsync(
                a => a.AssetId == asset.Id &&
                     a.AlertType == AlertType.ExceededPermanence &&
                     a.Status == AlertStatus.Active,
                cancellationToken)).Any();

            if (hasActiveAlert) continue;

            await alertRepository.AddAsync(new Alert
            {
                AssetId = asset.Id,
                AlertType = AlertType.ExceededPermanence,
                Severity = AlertSeverity.Medium,
                Message = $"El activo {asset.AssetTag} lleva fuera del campus más de {threshold.TotalHours:0} horas."
            }, cancellationToken);
            created++;
        }

        if (created > 0) await unitOfWork.SaveChangesAsync(cancellationToken);
        return created;
    }

    private static AlertDto ToDto(Alert a) => new(
        a.Id, a.AssetId, a.Asset?.AssetTag, a.AlertType, a.Severity, a.Status,
        a.Message, a.ResolvedAt, a.ResolvedBy, a.CreatedAt);
}

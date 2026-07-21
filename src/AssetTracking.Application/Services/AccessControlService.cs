using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.AccessControl;
using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Domain.Interfaces;

namespace AssetTracking.Application.Services;

public interface IAccessControlService
{
    Task<AccessValidationResult> ValidateAsync(
        ValidateAccessRequest request, AuditContext audit, CancellationToken cancellationToken = default);
}

public class AccessControlService(
    IPersonRepository personRepository,
    IAssetRepository assetRepository,
    IMovementService movementService,
    IRepository<Alert> alertRepository,
    IUnitOfWork unitOfWork,
    IAuditService auditService) : IAccessControlService
{
    public async Task<AccessValidationResult> ValidateAsync(
        ValidateAccessRequest request, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdentifierAsync(request.PersonIdentifier, cancellationToken);
        if (person is null)
        {
            await DenyAsync(null, audit, "No existe una persona registrada con ese identificador.", cancellationToken);
            return new AccessValidationResult(false, "No existe una persona registrada con ese identificador.", "PERSON_NOT_FOUND", null);
        }

        if (!person.IsActive)
        {
            await DenyAsync(null, audit, $"La persona {person.FirstName} {person.LastName} está inactiva.", cancellationToken);
            return new AccessValidationResult(false, "La persona se encuentra inactiva.", "PERSON_INACTIVE", null);
        }

        var asset = await assetRepository.GetByIdentifierAsync(request.AssetIdentifier, cancellationToken);
        if (asset is null)
        {
            await CreateAlertAsync(null, AlertType.UnregisteredAsset, AlertSeverity.High,
                $"Intento de acceso con activo no registrado (identificador: {request.AssetIdentifier}).", cancellationToken);
            await DenyAsync(null, audit, "El activo no está registrado en el sistema.", cancellationToken);
            return new AccessValidationResult(false, "El activo no está registrado en el sistema.", "ASSET_NOT_FOUND", null);
        }

        if (asset.Status == AssetStatusType.Reported)
        {
            await CreateAlertAsync(asset.Id, AlertType.ReportedAsset, AlertSeverity.Critical,
                $"Intento de movimiento de un activo reportado: {asset.AssetTag}.", cancellationToken);
            await DenyAsync(asset.Id, audit, "El activo se encuentra reportado.", cancellationToken);
            return new AccessValidationResult(false, "El activo se encuentra reportado.", "ASSET_REPORTED", null);
        }

        if (asset.OwnerId is null)
        {
            await CreateAlertAsync(asset.Id, AlertType.AssetWithoutOwner, AlertSeverity.Low,
                $"El activo {asset.AssetTag} no tiene propietario asignado.", cancellationToken);
        }

        var stateCheck = Application.Services.MovementService.ValidateStateTransition(asset, request.MovementType);
        if (!stateCheck.IsSuccess)
        {
            if (request.MovementType == MovementType.Exit)
            {
                await CreateAlertAsync(asset.Id, AlertType.UnauthorizedExit, AlertSeverity.Medium,
                    $"Intento de salida no autorizada del activo {asset.AssetTag}: {stateCheck.Error}", cancellationToken);
            }

            await DenyAsync(asset.Id, audit, stateCheck.Error!, cancellationToken);
            return new AccessValidationResult(false, stateCheck.Error, stateCheck.ErrorCode, null);
        }

        var movementResult = await movementService.RegisterAsync(
            new DTOs.Movements.RegisterMovementRequest(
                asset.AssetTag, person.DocumentNumber, request.LocationId, request.MovementType,
                ValidationMethod: request.ValidationMethod),
            audit,
            cancellationToken);

        if (!movementResult.IsSuccess)
        {
            return new AccessValidationResult(false, movementResult.Error, movementResult.ErrorCode, null);
        }

        await auditService.LogAsync(
            audit.UserId, AuditAction.Create, "AccessControl", asset.Id,
            $"Acceso autorizado: {request.MovementType} de {asset.AssetTag} por {person.FirstName} {person.LastName}",
            audit.IpAddress, audit.UserAgent, result: true, cancellationToken: cancellationToken);

        return new AccessValidationResult(true, null, null, movementResult.Value);
    }

    private async Task DenyAsync(Guid? assetId, AuditContext audit, string reason, CancellationToken cancellationToken) =>
        await auditService.LogAsync(
            audit.UserId, AuditAction.AccessDenied, "AccessControl", assetId,
            reason, audit.IpAddress, audit.UserAgent, result: false, cancellationToken: cancellationToken);

    private async Task CreateAlertAsync(
        Guid? assetId, AlertType alertType, AlertSeverity severity, string message, CancellationToken cancellationToken)
    {
        var alert = new Alert
        {
            AssetId = assetId,
            AlertType = alertType,
            Severity = severity,
            Message = message
        };

        await alertRepository.AddAsync(alert, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

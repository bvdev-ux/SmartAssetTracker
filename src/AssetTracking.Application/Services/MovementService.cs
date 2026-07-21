using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Movements;
using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Domain.Interfaces;

namespace AssetTracking.Application.Services;

public interface IMovementService
{
    Task<Result<MovementDto>> RegisterAsync(
        RegisterMovementRequest request, AuditContext audit, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<MovementDto>>> GetHistoryByAssetAsync(
        Guid assetId, int page, int pageSize, CancellationToken cancellationToken = default);
}

public class MovementService(
    IAssetRepository assetRepository,
    IPersonRepository personRepository,
    ILocationRepository locationRepository,
    IAssetMovementRepository movementRepository,
    IUnitOfWork unitOfWork,
    IAuditService auditService) : IMovementService
{
    public async Task<Result<MovementDto>> RegisterAsync(
        RegisterMovementRequest request, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdentifierAsync(request.AssetIdentifier, cancellationToken);
        if (asset is null) return Result<MovementDto>.Failure("Activo no encontrado.", "ASSET_NOT_FOUND");

        var person = await personRepository.GetByIdentifierAsync(request.PersonIdentifier, cancellationToken);
        if (person is null) return Result<MovementDto>.Failure("Persona no encontrada.", "PERSON_NOT_FOUND");

        var location = await locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        if (location is null) return Result<MovementDto>.Failure("Ubicación no encontrada.", "LOCATION_NOT_FOUND");

        var stateCheck = ValidateStateTransition(asset, request.MovementType);
        if (!stateCheck.IsSuccess)
        {
            return Result<MovementDto>.Failure(stateCheck.Error!, stateCheck.ErrorCode);
        }

        ApplyMovementEffects(asset, request.MovementType, location.Id);

        var movement = new AssetMovement
        {
            AssetId = asset.Id,
            PersonId = person.Id,
            LocationId = location.Id,
            MovementType = request.MovementType,
            Notes = request.Notes,
            ValidationMethod = request.ValidationMethod
        };

        assetRepository.Update(asset);
        await movementRepository.AddAsync(movement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Create, nameof(AssetMovement), movement.Id,
            $"Registró {request.MovementType} del activo {asset.AssetTag}", audit.IpAddress, audit.UserAgent,
            cancellationToken: cancellationToken);

        return Result<MovementDto>.Success(new MovementDto(
            movement.Id, asset.Id, asset.AssetTag, asset.Category?.Name ?? string.Empty,
            person.Id, $"{person.FirstName} {person.LastName}",
            location.Id, location.Name, movement.MovementType, movement.OccurredAt,
            movement.Notes, movement.ValidationMethod));
    }

    public async Task<Result<PagedResult<MovementDto>>> GetHistoryByAssetAsync(
        Guid assetId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null) return Result<PagedResult<MovementDto>>.Failure("Activo no encontrado.", "ASSET_NOT_FOUND");

        var (items, total) = await movementRepository.GetHistoryByAssetAsync(assetId, page, pageSize, cancellationToken);

        var dto = new PagedResult<MovementDto>
        {
            Items = items.Select(m => new MovementDto(
                m.Id, m.AssetId, m.Asset.AssetTag, m.Asset.Category.Name,
                m.PersonId, $"{m.Person.FirstName} {m.Person.LastName}",
                m.LocationId, m.Location.Name,
                m.MovementType, m.OccurredAt, m.Notes, m.ValidationMethod)).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };

        return Result<PagedResult<MovementDto>>.Success(dto);
    }

    internal static Result ValidateStateTransition(Asset asset, MovementType movementType)
    {
        switch (movementType)
        {
            case MovementType.Entry:
            case MovementType.ReEntry:
                return asset.IsInsideCampus
                    ? Result.Failure("El activo ya se encuentra dentro del campus.", "ASSET_ALREADY_INSIDE")
                    : Result.Success();
            case MovementType.Exit:
                return !asset.IsInsideCampus
                    ? Result.Failure("El activo ya se encuentra fuera del campus.", "ASSET_ALREADY_OUTSIDE")
                    : Result.Success();
            case MovementType.Loan:
                return asset.Status == AssetStatusType.InUse
                    ? Result.Failure("El activo ya está en préstamo.", "ASSET_ALREADY_LOANED")
                    : Result.Success();
            case MovementType.Return:
                return asset.Status != AssetStatusType.InUse
                    ? Result.Failure("El activo no se encuentra en préstamo.", "ASSET_NOT_LOANED")
                    : Result.Success();
            default:
                return Result.Success();
        }
    }

    internal static void ApplyMovementEffects(Asset asset, MovementType movementType, Guid locationId)
    {
        switch (movementType)
        {
            case MovementType.Entry:
            case MovementType.ReEntry:
                asset.IsInsideCampus = true;
                asset.CurrentLocationId = locationId;
                break;
            case MovementType.Exit:
                asset.IsInsideCampus = false;
                asset.CurrentLocationId = locationId;
                break;
            case MovementType.Loan:
                asset.Status = AssetStatusType.InUse;
                asset.CurrentLocationId = locationId;
                break;
            case MovementType.Return:
                asset.Status = AssetStatusType.Available;
                asset.CurrentLocationId = locationId;
                break;
        }
    }
}

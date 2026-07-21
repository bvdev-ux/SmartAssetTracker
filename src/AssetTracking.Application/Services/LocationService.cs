using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Locations;
using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Domain.Interfaces;

namespace AssetTracking.Application.Services;

public interface ILocationService
{
    Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LocationTreeDto>> GetTreeAsync(CancellationToken cancellationToken = default);

    Task<Result<LocationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<LocationDto>> CreateAsync(
        CreateLocationRequest request, AuditContext audit, CancellationToken cancellationToken = default);

    Task<Result<LocationDto>> UpdateAsync(
        Guid id, UpdateLocationRequest request, AuditContext audit, CancellationToken cancellationToken = default);

    Task<Result> DeactivateAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default);
}

public class LocationService(
    ILocationRepository locationRepository,
    IUnitOfWork unitOfWork,
    IAuditService auditService) : ILocationService
{
    public async Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var locations = await locationRepository.GetAllAsync(cancellationToken);
        var namesById = locations.ToDictionary(l => l.Id, l => l.Name);

        return locations
            .Select(l => ToDto(l, l.ParentLocationId.HasValue ? namesById.GetValueOrDefault(l.ParentLocationId.Value) : null))
            .OrderBy(l => l.Name)
            .ToList();
    }

    public async Task<IReadOnlyList<LocationTreeDto>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        var locations = await locationRepository.GetAllWithHierarchyAsync(cancellationToken);
        var byParent = locations.ToLookup(l => l.ParentLocationId);

        LocationTreeDto Build(Location l) => new(
            l.Id, l.Name, l.Code, l.LocationType, l.IsActive,
            byParent[l.Id].OrderBy(c => c.Name).Select(Build).ToList());

        return byParent[null].OrderBy(l => l.Name).Select(Build).ToList();
    }

    public async Task<Result<LocationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await locationRepository.GetByIdAsync(id, cancellationToken);
        if (location is null) return Result<LocationDto>.Failure("Ubicación no encontrada.", "LOCATION_NOT_FOUND");

        string? parentName = null;
        if (location.ParentLocationId.HasValue)
        {
            var parent = await locationRepository.GetByIdAsync(location.ParentLocationId.Value, cancellationToken);
            parentName = parent?.Name;
        }

        return Result<LocationDto>.Success(ToDto(location, parentName));
    }

    public async Task<Result<LocationDto>> CreateAsync(
        CreateLocationRequest request, AuditContext audit, CancellationToken cancellationToken = default)
    {
        if (await locationRepository.ExistsByCodeAsync(request.Code, null, cancellationToken))
        {
            return Result<LocationDto>.Failure("Ya existe una ubicación con ese código.", "LOCATION_DUPLICATE");
        }

        if (request.ParentLocationId.HasValue &&
            await locationRepository.GetByIdAsync(request.ParentLocationId.Value, cancellationToken) is null)
        {
            return Result<LocationDto>.Failure("La ubicación padre no existe.", "LOCATION_PARENT_NOT_FOUND");
        }

        var location = new Location
        {
            Name = request.Name,
            Code = request.Code,
            LocationType = request.LocationType,
            ParentLocationId = request.ParentLocationId
        };

        await locationRepository.AddAsync(location, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Create, nameof(Location), location.Id,
            $"Registró la ubicación {location.Name}", audit.IpAddress, audit.UserAgent,
            cancellationToken: cancellationToken);

        return Result<LocationDto>.Success(ToDto(location, null));
    }

    public async Task<Result<LocationDto>> UpdateAsync(
        Guid id, UpdateLocationRequest request, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var location = await locationRepository.GetByIdAsync(id, cancellationToken);
        if (location is null) return Result<LocationDto>.Failure("Ubicación no encontrada.", "LOCATION_NOT_FOUND");

        if (await locationRepository.ExistsByCodeAsync(request.Code, id, cancellationToken))
        {
            return Result<LocationDto>.Failure("Ya existe una ubicación con ese código.", "LOCATION_DUPLICATE");
        }

        if (request.ParentLocationId == id)
        {
            return Result<LocationDto>.Failure("Una ubicación no puede ser su propio padre.", "LOCATION_INVALID_PARENT");
        }

        location.Name = request.Name;
        location.Code = request.Code;
        location.LocationType = request.LocationType;
        location.ParentLocationId = request.ParentLocationId;
        location.IsActive = request.IsActive;

        locationRepository.Update(location);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Update, nameof(Location), location.Id,
            ipAddress: audit.IpAddress, userAgent: audit.UserAgent, cancellationToken: cancellationToken);

        return Result<LocationDto>.Success(ToDto(location, null));
    }

    public async Task<Result> DeactivateAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var location = await locationRepository.GetByIdAsync(id, cancellationToken);
        if (location is null) return Result.Failure("Ubicación no encontrada.", "LOCATION_NOT_FOUND");

        location.IsActive = false;
        locationRepository.Update(location);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Delete, nameof(Location), location.Id,
            ipAddress: audit.IpAddress, userAgent: audit.UserAgent, cancellationToken: cancellationToken);

        return Result.Success();
    }

    private static LocationDto ToDto(Location l, string? parentName) => new(
        l.Id, l.Name, l.Code, l.LocationType, l.ParentLocationId, parentName, l.IsActive);
}

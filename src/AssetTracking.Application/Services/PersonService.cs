using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Movements;
using AssetTracking.Application.DTOs.People;
using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Domain.Interfaces;

namespace AssetTracking.Application.Services;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> GetPagedAsync(
        string? search, PersonType? personType, bool? isActive, int page, int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<PersonDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<PersonDto>> CreateAsync(
        CreatePersonRequest request, AuditContext audit, CancellationToken cancellationToken = default);

    Task<Result<PersonDto>> UpdateAsync(
        Guid id, UpdatePersonRequest request, AuditContext audit, CancellationToken cancellationToken = default);

    Task<Result> DeactivateAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<MovementDto>>> GetMovementHistoryAsync(
        Guid personId, int page, int pageSize, CancellationToken cancellationToken = default);
}

public class PersonService(
    IPersonRepository personRepository,
    IAssetMovementRepository movementRepository,
    IUnitOfWork unitOfWork,
    IAuditService auditService) : IPersonService
{
    public async Task<PagedResult<PersonDto>> GetPagedAsync(
        string? search, PersonType? personType, bool? isActive, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await personRepository.SearchAsync(
            search, personType, isActive, page, pageSize, cancellationToken);

        return new PagedResult<PersonDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Result<PersonDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(id, cancellationToken);
        return person is null
            ? Result<PersonDto>.Failure("Persona no encontrada.", "PERSON_NOT_FOUND")
            : Result<PersonDto>.Success(ToDto(person));
    }

    public async Task<Result<PersonDto>> CreateAsync(
        CreatePersonRequest request, AuditContext audit, CancellationToken cancellationToken = default)
    {
        if (await personRepository.ExistsByDocumentNumberAsync(request.DocumentNumber, null, cancellationToken))
        {
            return Result<PersonDto>.Failure("Ya existe una persona con ese documento.", "PERSON_DUPLICATE");
        }

        var person = new Person
        {
            DocumentNumber = request.DocumentNumber,
            UniversityCode = request.UniversityCode,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Career = request.Career,
            Faculty = request.Faculty,
            CardQrCode = request.CardQrCode,
            PersonType = request.PersonType
        };

        await personRepository.AddAsync(person, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Create, nameof(Person), person.Id,
            $"Registró a {person.FirstName} {person.LastName}",
            audit.IpAddress, audit.UserAgent, cancellationToken: cancellationToken);

        return Result<PersonDto>.Success(ToDto(person));
    }

    public async Task<Result<PersonDto>> UpdateAsync(
        Guid id, UpdatePersonRequest request, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(id, cancellationToken);
        if (person is null) return Result<PersonDto>.Failure("Persona no encontrada.", "PERSON_NOT_FOUND");

        person.FirstName = request.FirstName;
        person.LastName = request.LastName;
        person.Email = request.Email;
        person.Phone = request.Phone;
        person.Career = request.Career;
        person.Faculty = request.Faculty;
        person.CardQrCode = request.CardQrCode;
        person.PersonType = request.PersonType;
        person.IsActive = request.IsActive;

        personRepository.Update(person);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Update, nameof(Person), person.Id,
            ipAddress: audit.IpAddress, userAgent: audit.UserAgent, cancellationToken: cancellationToken);

        return Result<PersonDto>.Success(ToDto(person));
    }

    public async Task<Result> DeactivateAsync(Guid id, AuditContext audit, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(id, cancellationToken);
        if (person is null) return Result.Failure("Persona no encontrada.", "PERSON_NOT_FOUND");

        person.IsActive = false;
        personRepository.Update(person);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            audit.UserId, AuditAction.Delete, nameof(Person), person.Id,
            ipAddress: audit.IpAddress, userAgent: audit.UserAgent, cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PagedResult<MovementDto>>> GetMovementHistoryAsync(
        Guid personId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(personId, cancellationToken);
        if (person is null) return Result<PagedResult<MovementDto>>.Failure("Persona no encontrada.", "PERSON_NOT_FOUND");

        var (items, total) = await movementRepository.GetHistoryByPersonAsync(personId, page, pageSize, cancellationToken);

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

    private static PersonDto ToDto(Person p) => new(
        p.Id, p.DocumentNumber, p.UniversityCode, p.FirstName, p.LastName,
        p.Email, p.Phone, p.Career, p.Faculty, p.CardQrCode, p.PersonType, p.IsActive);
}

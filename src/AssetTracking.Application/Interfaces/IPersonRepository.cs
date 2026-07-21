using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.Interfaces;

public interface IPersonRepository : Domain.Interfaces.IRepository<Person>
{
    Task<Person?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Person> Items, int TotalCount)> SearchAsync(
        string? search,
        PersonType? personType,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByDocumentNumberAsync(
        string documentNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}

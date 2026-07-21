using AssetTracking.Application.Interfaces;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Infrastructure.Repositories;

public class PersonRepository(ApplicationDbContext context) : Repository<Person>(context), IPersonRepository
{
    public async Task<Person?> GetByIdentifierAsync(
        string identifier,
        CancellationToken cancellationToken = default) =>
        await Context.People.FirstOrDefaultAsync(
            p => p.DocumentNumber == identifier ||
                 p.UniversityCode == identifier ||
                 p.CardQrCode == identifier,
            cancellationToken);

    public async Task<(IReadOnlyList<Person> Items, int TotalCount)> SearchAsync(
        string? search,
        PersonType? personType,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.People.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term) ||
                p.DocumentNumber.ToLower().Contains(term) ||
                (p.UniversityCode != null && p.UniversityCode.ToLower().Contains(term)));
        }

        if (personType.HasValue) query = query.Where(p => p.PersonType == personType.Value);
        if (isActive.HasValue) query = query.Where(p => p.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByDocumentNumberAsync(
        string documentNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default) =>
        await Context.People.AnyAsync(
            p => p.DocumentNumber == documentNumber && (excludeId == null || p.Id != excludeId),
            cancellationToken);
}

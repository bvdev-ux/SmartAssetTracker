using System.Linq.Expressions;

namespace AssetTracking.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);

    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>>? predicate,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate,
        CancellationToken cancellationToken = default);
}

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IAuditService
{
    Task LogAsync(
        Guid? userId,
        Domain.Enums.AuditAction action,
        string entityName,
        Guid? entityId,
        string? details = null,
        string? ipAddress = null,
        string? userAgent = null,
        bool result = true,
        CancellationToken cancellationToken = default);
}

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string role, IEnumerable<string> permissions);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

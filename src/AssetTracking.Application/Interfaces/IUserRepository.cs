namespace AssetTracking.Application.Interfaces;

public interface IUserRepository
{
    Task<Domain.Entities.User?> GetByEmailWithRoleAsync(
        string email,
        CancellationToken cancellationToken = default);
}

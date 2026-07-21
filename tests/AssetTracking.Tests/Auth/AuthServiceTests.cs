using AssetTracking.Application.DTOs.Auth;
using AssetTracking.Application.Services;
using AssetTracking.Domain.Entities;
using AssetTracking.Infrastructure.Repositories;
using AssetTracking.Infrastructure.Services;
using AssetTracking.Tests.Common;
using Xunit;

namespace AssetTracking.Tests.Auth;

public class AuthServiceTests
{
    private static async Task<(AuthService Service, User User)> CreateServiceWithSeededUserAsync(string password)
    {
        var context = TestDbContextFactory.Create();
        var hasher = new PasswordHasher();

        var role = new Role { Name = "Admin" };
        var user = new User
        {
            Email = "admin@upla.edu.pe",
            PasswordHash = hasher.Hash(password),
            FullName = "Admin User",
            RoleId = role.Id,
            Role = role
        };

        context.Roles.Add(role);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new AuthService(
            new UserRepository(context),
            new TokenService(TestDbContextFactory.CreateJwtConfiguration()),
            hasher,
            new AuditService(context),
            new UnitOfWork(context));

        return (service, user);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessWithToken()
    {
        var (service, user) = await CreateServiceWithSeededUserAsync("Sup3rSecret!");

        var result = await service.LoginAsync(new LoginRequest(user.Email, "Sup3rSecret!"));

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.AccessToken));
        Assert.Equal(user.Email, result.Value.User.Email);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFailure()
    {
        var (service, user) = await CreateServiceWithSeededUserAsync("Sup3rSecret!");

        var result = await service.LoginAsync(new LoginRequest(user.Email, "WrongPassword"));

        Assert.False(result.IsSuccess);
        Assert.Equal("AUTH_INVALID", result.ErrorCode);
    }

    [Fact]
    public async Task LoginAsync_WithUnknownEmail_ReturnsFailure()
    {
        var (service, _) = await CreateServiceWithSeededUserAsync("Sup3rSecret!");

        var result = await service.LoginAsync(new LoginRequest("unknown@upla.edu.pe", "Sup3rSecret!"));

        Assert.False(result.IsSuccess);
        Assert.Equal("AUTH_INVALID", result.ErrorCode);
    }
}

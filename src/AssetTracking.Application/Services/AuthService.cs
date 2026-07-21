using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Auth;
using AssetTracking.Domain.Interfaces;

namespace AssetTracking.Application.Services;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public class AuthService : IAuthService
{
    private readonly Interfaces.IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        Interfaces.IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IAuditService auditService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _auditService = auditService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailWithRoleAsync(request.Email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure("Credenciales inválidas.", "AUTH_INVALID");
        }

        var permissions = user.Role?.RolePermissions
            .Select(rp => rp.Permission.Code)
            .ToList() ?? [];

        var token = _tokenService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.Role?.Name ?? "User",
            permissions);

        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            user.Id,
            Domain.Enums.AuditAction.Login,
            nameof(Domain.Entities.User),
            user.Id,
            cancellationToken: cancellationToken);

        var response = new LoginResponse(
            token,
            DateTime.UtcNow.AddHours(8),
            new UserDto(user.Id, user.Email, user.FullName, user.Role?.Name ?? "User", permissions));

        return Result<LoginResponse>.Success(response);
    }
}

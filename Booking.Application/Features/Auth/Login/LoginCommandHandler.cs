using Booking.Application.Abstractions.LogIn;
using Booking.Application.Abstractions.Security;
using Booking.Application.Abstractions.UserRegister;
using Booking.Application.Common.Exceptions;
using MediatR;

namespace Booking.Application.Features.Auth.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthManager _authManager;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAuthManager authManager)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authManager = authManager;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var email = request.Request.Email.Trim().ToLower();
        var password = request.Request.Password;

        var user = await _userRepository.GetByEmailWithRolesAsync(email, ct);

        if (user is null)
            throw new UnauthorizedException("Invalid credentials");

        if (user.IsDeleted)
            throw new UnauthorizedException("This account has been deleted.");

        if (!user.IsActive)
            throw new UnauthorizedException("Your account has been suspended.");

        if (!_passwordHasher.Verify(password, user.Password))
            throw new UnauthorizedException("Invalid credentials");

        var token = _authManager.GenerateToken(user);

        return new LoginResponse(
            token,
            "Bearer",
            _authManager.GetExpiresSeconds()
        );
    }
}
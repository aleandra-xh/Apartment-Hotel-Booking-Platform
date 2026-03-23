using Booking.Application.Abstractions.Security;
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Abstractions.UserRegister;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Roles;
using Booking.Domain.UserRoles;
using Booking.Domain.Users;
using MediatR;
using Booking.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Users.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IGenericRepository<Role> roleRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedDto = request.UserDto with
        {
            Email = request.UserDto.Email.Trim().ToLower()
        };

        var isUnique = await _userRepository
            .IsEmailUnique(normalizedDto.Email, cancellationToken);

        if (!isUnique)
            throw new ConflictException("Email already exists.");

        var passwordHash = _passwordHasher.Hash(normalizedDto.Password);

        var defaultRole = await _roleRepository
            .FirstOrDefaultAsync(r => r.IsDefault, cancellationToken);

        if (defaultRole is null)
            throw new ConflictException("No default role found. Seed a default role first.");

        var user = User.CreateUser(normalizedDto, passwordHash);

        var userRole = UserRole.CreateDefaultUserRole(user, defaultRole);
        user.UserRoles.Add(userRole);

        await _userRepository.AddAsync(user, cancellationToken);

        try
        {
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new ConflictException(ex.InnerException?.Message ?? ex.Message);
        }

        await _emailService.SendAsync(
            new EmailMessage(
                user.Email,
                "Welcome to Booking App",
                $"Hello {user.FirstName}, your account has been created successfully."
            ),
            cancellationToken);

        return user.Id;
    }
}
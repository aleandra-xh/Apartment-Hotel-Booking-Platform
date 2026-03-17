using System;
using System.Collections.Generic;
using System.Text;

using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Abstractions.UserRegister;
using MediatR;

namespace Booking.Application.Features.Users.ChangePassword;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        var isCurrentPasswordValid = _passwordHasher.Verify(
            request.Request.CurrentPassword,
            user.Password);

        if (!isCurrentPasswordValid)
            throw new ConflictException("Current password is incorrect.");

        var newPasswordHash = _passwordHasher.Hash(request.Request.NewPassword);

        user.UpdatePassword(newPasswordHash);

        await _userRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

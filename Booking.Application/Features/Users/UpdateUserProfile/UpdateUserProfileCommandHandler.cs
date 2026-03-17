
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Abstractions.UserRegister;
using MediatR;


namespace Booking.Application.Features.Users.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateUserProfileCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        if (request.Request.FirstName is not null)
            user.FirstName = request.Request.FirstName;

        if (request.Request.LastName is not null)
            user.LastName = request.Request.LastName;

        if (request.Request.PhoneNumber is not null)
            user.PhoneNumber = request.Request.PhoneNumber;

        user.LastModifiedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
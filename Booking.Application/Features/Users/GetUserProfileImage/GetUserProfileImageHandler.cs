
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Abstractions.UserRegister;
using MediatR;

namespace Booking.Application.Features.Users.GetUserProfileImage;
public sealed class GetUserProfileImageQueryHandler
    : IRequestHandler<GetUserProfileImageQuery, GetUserProfileImageResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUserProfileImageQueryHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetUserProfileImageResponse> Handle(GetUserProfileImageQuery request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        return new GetUserProfileImageResponse(user.ProfileImageUrl);
    }
}
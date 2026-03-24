
using Booking.Application.Abstractions.Security;
using Booking.Application.Abstractions.UserRegister;
using Booking.Application.Common.Exceptions;
using MediatR;

namespace Booking.Application.Features.Users.GetMyProfile;

public sealed class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, GetMyProfileResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyProfileQueryHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetMyProfileResponse> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var user = await _userRepository.GetByIdWithRolesAsync(userId, ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        return new GetMyProfileResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.IsActive,
            user.CreatedAt,
            user.LastModifiedAt,
            user.UserRoles
                .Select(ur => ur.Role.Name.ToString())
                .Distinct()
                .ToList()
        );
    }
}
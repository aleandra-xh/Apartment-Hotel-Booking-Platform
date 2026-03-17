
using MediatR;

namespace Booking.Application.Features.Users.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    UpdateUserProfileRequest Request
) : IRequest<Unit>;
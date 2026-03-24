
using MediatR;

namespace Booking.Application.Features.Users.GetMyProfile;

public sealed record GetMyProfileQuery : IRequest<GetMyProfileResponse>;

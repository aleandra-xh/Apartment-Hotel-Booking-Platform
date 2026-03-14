
using MediatR;

namespace Booking.Application.Features.Users.GetUserProfileImage;

public sealed record GetUserProfileImageQuery : IRequest<GetUserProfileImageResponse>;
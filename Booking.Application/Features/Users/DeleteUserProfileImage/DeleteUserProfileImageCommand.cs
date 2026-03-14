
using MediatR;

namespace Booking.Application.Features.Users.DeleteUserProfileImage;
public sealed record DeleteUserProfileImageCommand : IRequest<Unit>;
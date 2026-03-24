
using MediatR;

namespace Booking.Application.Features.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Unit>;
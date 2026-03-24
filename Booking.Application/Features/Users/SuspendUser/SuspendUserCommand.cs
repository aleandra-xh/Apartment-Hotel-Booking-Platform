
using MediatR;

namespace Booking.Application.Features.Users.SuspendUser;

public sealed record SuspendUserCommand(Guid UserId) : IRequest<Unit>;
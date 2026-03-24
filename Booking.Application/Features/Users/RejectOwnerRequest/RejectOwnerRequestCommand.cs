
using MediatR;

namespace Booking.Application.Features.Users.RejectOwnerRequest;

public sealed record RejectOwnerRequestCommand(Guid UserId) : IRequest<Unit>;
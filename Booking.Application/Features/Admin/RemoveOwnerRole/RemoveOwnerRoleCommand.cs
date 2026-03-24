
using MediatR;

namespace Booking.Application.Features.Admin.RemoveOwnerRole;

public sealed record RemoveOwnerRoleCommand(Guid UserId) : IRequest<Unit>;
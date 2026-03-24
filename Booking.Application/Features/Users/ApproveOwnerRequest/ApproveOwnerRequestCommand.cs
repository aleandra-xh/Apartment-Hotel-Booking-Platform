
using MediatR;

namespace Booking.Application.Features.Users.ApproveOwnerRequest;

public sealed record ApproveOwnerRequestCommand(Guid UserId) : IRequest<Unit>;
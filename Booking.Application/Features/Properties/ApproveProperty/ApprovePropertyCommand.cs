
using MediatR;

namespace Booking.Application.Features.Properties.ApproveProperty;

public sealed record ApprovePropertyCommand(Guid PropertyId) : IRequest<Unit>;
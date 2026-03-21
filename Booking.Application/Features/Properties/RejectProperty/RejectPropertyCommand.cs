
using MediatR;

namespace Booking.Application.Features.Properties.RejectProperty;

public sealed record RejectPropertyCommand(Guid PropertyId) : IRequest<Unit>;

using MediatR;

namespace Booking.Application.Features.Properties.SuspendProperty;

public sealed record SuspendPropertyCommand(Guid PropertyId) : IRequest<Unit>;

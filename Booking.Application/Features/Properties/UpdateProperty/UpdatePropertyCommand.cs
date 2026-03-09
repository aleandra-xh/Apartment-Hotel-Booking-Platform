using MediatR;

namespace Booking.Application.Features.Properties.UpdateProperty;

public sealed record UpdatePropertyCommand(
    Guid PropertyId,
    UpdatePropertyRequest Request
) : IRequest<Unit>;

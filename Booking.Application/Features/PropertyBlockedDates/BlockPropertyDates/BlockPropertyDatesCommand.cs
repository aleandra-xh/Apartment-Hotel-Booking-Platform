
using MediatR;

namespace Booking.Application.Features.PropertyBlockedDates.BlockPropertyDates;
public sealed record BlockPropertyDatesCommand(
    BlockPropertyDatesRequest Request
) : IRequest<Guid>;
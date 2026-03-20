
using MediatR;

namespace Booking.Application.Features.PropertyBlockedDates.DeleteBlockedDate;

public sealed record DeleteBlockedDateCommand(Guid BlockedDateId) : IRequest<Unit>;
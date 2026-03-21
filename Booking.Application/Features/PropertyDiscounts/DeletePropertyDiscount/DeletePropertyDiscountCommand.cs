
using MediatR;

namespace Booking.Application.Features.PropertyDiscounts.DeletePropertyDiscount;

public sealed record DeletePropertyDiscountCommand(Guid DiscountId) : IRequest<Unit>;
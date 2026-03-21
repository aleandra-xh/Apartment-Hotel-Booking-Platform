
using MediatR;

namespace Booking.Application.Features.PropertyDiscounts.AddPropertyDiscount;

public sealed record AddPropertyDiscountCommand(
    AddPropertyDiscountRequest Request
) : IRequest<Guid>;
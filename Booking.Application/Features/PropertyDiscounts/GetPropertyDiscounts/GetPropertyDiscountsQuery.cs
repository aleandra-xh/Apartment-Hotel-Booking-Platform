
using MediatR;

namespace Booking.Application.Features.PropertyDiscounts.GetPropertyDiscounts;

public sealed record GetPropertyDiscountsQuery(Guid PropertyId)
    : IRequest<List<GetPropertyDiscountsResponse>>;
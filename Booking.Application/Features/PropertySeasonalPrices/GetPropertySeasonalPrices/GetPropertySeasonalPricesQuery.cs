
using MediatR;

namespace Booking.Application.Features.PropertySeasonalPrices.GetPropertySeasonalPrices;

public sealed record GetPropertySeasonalPricesQuery(Guid PropertyId)
    : IRequest<List<GetPropertySeasonalPricesResponse>>;
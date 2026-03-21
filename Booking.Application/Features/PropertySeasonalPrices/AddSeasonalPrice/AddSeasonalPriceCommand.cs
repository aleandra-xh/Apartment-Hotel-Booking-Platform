
using MediatR;

namespace Booking.Application.Features.PropertySeasonalPrices.AddSeasonalPrice;

public sealed record AddSeasonalPriceCommand(
    AddSeasonalPriceRequest Request
) : IRequest<Guid>;
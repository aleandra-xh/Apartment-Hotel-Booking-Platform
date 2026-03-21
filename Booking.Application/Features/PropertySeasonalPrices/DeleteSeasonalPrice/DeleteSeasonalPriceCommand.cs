
using MediatR;

namespace Booking.Application.Features.PropertySeasonalPrices.DeleteSeasonalPrice;

public sealed record DeleteSeasonalPriceCommand(Guid SeasonalPriceId) : IRequest<Unit>;
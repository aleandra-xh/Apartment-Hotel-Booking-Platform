using Booking.Application.Abstractions.PropertySeasonalPrices;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using MediatR;

namespace Booking.Application.Features.PropertySeasonalPrices.GetPropertySeasonalPrices;

public sealed class GetPropertySeasonalPricesQueryHandler
    : IRequestHandler<GetPropertySeasonalPricesQuery, List<GetPropertySeasonalPricesResponse>>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IPropertySeasonalPriceRepository _seasonalPriceRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPropertySeasonalPricesQueryHandler(
        IGenericRepository<Property> propertyRepository,
        IPropertySeasonalPriceRepository seasonalPriceRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _seasonalPriceRepository = seasonalPriceRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<GetPropertySeasonalPricesResponse>> Handle(GetPropertySeasonalPricesQuery request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to view seasonal prices for this property.");

        var seasonalPrices = await _seasonalPriceRepository.GetByPropertyIdAsync(request.PropertyId, ct);

        return seasonalPrices
            .Select(sp => new GetPropertySeasonalPricesResponse(
                sp.Id,
                sp.PropertyId,
                sp.StartDate,
                sp.EndDate,
                sp.PricePerNight,
                sp.Label,
                sp.CreatedAt
            ))
            .ToList();
    }
}
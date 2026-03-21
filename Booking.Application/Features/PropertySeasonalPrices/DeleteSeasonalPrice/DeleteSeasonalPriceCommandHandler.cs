using Booking.Application.Abstractions.PropertySeasonalPrices;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.PropertySeasonalPrices;
using MediatR;

namespace Booking.Application.Features.PropertySeasonalPrices.DeleteSeasonalPrice;

public sealed class DeleteSeasonalPriceCommandHandler : IRequestHandler<DeleteSeasonalPriceCommand, Unit>
{
    private readonly IPropertySeasonalPriceRepository _seasonalPriceRepository;
    private readonly IGenericRepository<PropertySeasonalPrice> _genericSeasonalPriceRepository;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteSeasonalPriceCommandHandler(
        IPropertySeasonalPriceRepository seasonalPriceRepository,
        IGenericRepository<PropertySeasonalPrice> genericSeasonalPriceRepository,
        IGenericRepository<Property> propertyRepository,
        ICurrentUserService currentUserService)
    {
        _seasonalPriceRepository = seasonalPriceRepository;
        _genericSeasonalPriceRepository = genericSeasonalPriceRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteSeasonalPriceCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var seasonalPrice = await _seasonalPriceRepository.GetByIdAsync(request.SeasonalPriceId, ct);

        if (seasonalPrice is null)
            throw new NotFoundException("Seasonal price not found.");

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == seasonalPrice.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to delete seasonal prices for this property.");

        _genericSeasonalPriceRepository.Remove(seasonalPrice);
        await _genericSeasonalPriceRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
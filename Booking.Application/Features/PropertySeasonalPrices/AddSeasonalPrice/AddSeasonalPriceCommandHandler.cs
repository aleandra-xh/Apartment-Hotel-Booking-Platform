using Booking.Application.Abstractions.PropertySeasonalPrices;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.PropertySeasonalPrices;
using MediatR;

namespace Booking.Application.Features.PropertySeasonalPrices.AddSeasonalPrice;

public sealed class AddSeasonalPriceCommandHandler : IRequestHandler<AddSeasonalPriceCommand, Guid>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IGenericRepository<PropertySeasonalPrice> _genericSeasonalPriceRepository;
    private readonly IPropertySeasonalPriceRepository _seasonalPriceRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddSeasonalPriceCommandHandler(
        IGenericRepository<Property> propertyRepository,
        IGenericRepository<PropertySeasonalPrice> genericSeasonalPriceRepository,
        IPropertySeasonalPriceRepository seasonalPriceRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _genericSeasonalPriceRepository = genericSeasonalPriceRepository;
        _seasonalPriceRepository = seasonalPriceRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(AddSeasonalPriceCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.Request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to add seasonal prices for this property.");

        var hasOverlap = await _seasonalPriceRepository.HasOverlappingSeasonalPriceAsync(
            request.Request.PropertyId,
            request.Request.StartDate,
            request.Request.EndDate,
            ct);

        if (hasOverlap)
            throw new ConflictException("This property already has a seasonal price that overlaps with the selected range.");

        var seasonalPrice = new PropertySeasonalPrice
        {
            Id = Guid.NewGuid(),
            PropertyId = request.Request.PropertyId,
            StartDate = request.Request.StartDate.Date,
            EndDate = request.Request.EndDate.Date,
            PricePerNight = request.Request.PricePerNight,
            Label = request.Request.Label,
            CreatedAt = DateTime.UtcNow
        };

        await _genericSeasonalPriceRepository.AddAsync(seasonalPrice, ct);
        await _genericSeasonalPriceRepository.SaveChangesAsync(ct);

        return seasonalPrice.Id;
    }
}
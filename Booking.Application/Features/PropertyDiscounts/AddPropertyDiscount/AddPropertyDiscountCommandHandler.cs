
using Booking.Application.Abstractions.PropertyDiscounts;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.PropertyDiscounts;
using MediatR;

namespace Booking.Application.Features.PropertyDiscounts.AddPropertyDiscount;

public sealed class AddPropertyDiscountCommandHandler : IRequestHandler<AddPropertyDiscountCommand, Guid>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IGenericRepository<PropertyDiscount> _genericDiscountRepository;
    private readonly IPropertyDiscountRepository _discountRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddPropertyDiscountCommandHandler(
        IGenericRepository<Property> propertyRepository,
        IGenericRepository<PropertyDiscount> genericDiscountRepository,
        IPropertyDiscountRepository discountRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _genericDiscountRepository = genericDiscountRepository;
        _discountRepository = discountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(AddPropertyDiscountCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.Request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to add discounts for this property.");

        var hasOverlap = await _discountRepository.HasOverlappingDiscountAsync(
            request.Request.PropertyId,
            request.Request.StartDate,
            request.Request.EndDate,
            ct);

        if (hasOverlap)
            throw new ConflictException("This property already has a discount that overlaps with the selected range.");

        var discount = new PropertyDiscount
        {
            Id = Guid.NewGuid(),
            PropertyId = request.Request.PropertyId,
            StartDate = request.Request.StartDate.Date,
            EndDate = request.Request.EndDate.Date,
            Percentage = request.Request.Percentage,
            Label = request.Request.Label,
            CreatedAt = DateTime.UtcNow
        };

        await _genericDiscountRepository.AddAsync(discount, ct);
        await _genericDiscountRepository.SaveChangesAsync(ct);

        return discount.Id;
    }
}
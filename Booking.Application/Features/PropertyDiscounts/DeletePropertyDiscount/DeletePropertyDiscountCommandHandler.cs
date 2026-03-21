
using Booking.Application.Abstractions.PropertyDiscounts;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.PropertyDiscounts;
using MediatR;

namespace Booking.Application.Features.PropertyDiscounts.DeletePropertyDiscount;

public sealed class DeletePropertyDiscountCommandHandler : IRequestHandler<DeletePropertyDiscountCommand, Unit>
{
    private readonly IPropertyDiscountRepository _discountRepository;
    private readonly IGenericRepository<PropertyDiscount> _genericDiscountRepository;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeletePropertyDiscountCommandHandler(
        IPropertyDiscountRepository discountRepository,
        IGenericRepository<PropertyDiscount> genericDiscountRepository,
        IGenericRepository<Property> propertyRepository,
        ICurrentUserService currentUserService)
    {
        _discountRepository = discountRepository;
        _genericDiscountRepository = genericDiscountRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeletePropertyDiscountCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var discount = await _discountRepository.GetByIdAsync(request.DiscountId, ct);

        if (discount is null)
            throw new NotFoundException("Property discount not found.");

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == discount.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to delete discounts for this property.");

        _genericDiscountRepository.Remove(discount);
        await _genericDiscountRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
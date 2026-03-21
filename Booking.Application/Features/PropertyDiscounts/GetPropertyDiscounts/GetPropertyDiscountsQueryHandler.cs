
using Booking.Application.Abstractions.PropertyDiscounts;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using MediatR;

namespace Booking.Application.Features.PropertyDiscounts.GetPropertyDiscounts;

public sealed class GetPropertyDiscountsQueryHandler
    : IRequestHandler<GetPropertyDiscountsQuery, List<GetPropertyDiscountsResponse>>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IPropertyDiscountRepository _discountRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPropertyDiscountsQueryHandler(
        IGenericRepository<Property> propertyRepository,
        IPropertyDiscountRepository discountRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _discountRepository = discountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<GetPropertyDiscountsResponse>> Handle(GetPropertyDiscountsQuery request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to view discounts for this property.");

        var discounts = await _discountRepository.GetByPropertyIdAsync(request.PropertyId, ct);

        return discounts
            .Select(d => new GetPropertyDiscountsResponse(
                d.Id,
                d.PropertyId,
                d.StartDate,
                d.EndDate,
                d.Percentage,
                d.Label,
                d.CreatedAt
            ))
            .ToList();
    }
}
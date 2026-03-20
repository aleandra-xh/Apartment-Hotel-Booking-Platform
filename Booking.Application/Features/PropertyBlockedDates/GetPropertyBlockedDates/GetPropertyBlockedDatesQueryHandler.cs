
using Booking.Application.Abstractions.PropertyBlockedDates;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using MediatR;

namespace Booking.Application.Features.PropertyBlockedDates.GetPropertyBlockedDates;

public sealed class GetPropertyBlockedDatesQueryHandler
    : IRequestHandler<GetPropertyBlockedDatesQuery, List<GetPropertyBlockedDatesResponse>>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IPropertyBlockedDateRepository _blockedDateRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPropertyBlockedDatesQueryHandler(
        IGenericRepository<Property> propertyRepository,
        IPropertyBlockedDateRepository blockedDateRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _blockedDateRepository = blockedDateRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<GetPropertyBlockedDatesResponse>> Handle(GetPropertyBlockedDatesQuery request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to view blocked dates for this property.");

        var blockedDates = await _blockedDateRepository.GetByPropertyIdAsync(request.PropertyId, ct);

        return blockedDates
            .Select(b => new GetPropertyBlockedDatesResponse(
                b.Id,
                b.PropertyId,
                b.StartDate,
                b.EndDate,
                b.Reason,
                b.CreatedAt
            ))
            .ToList();
    }
}
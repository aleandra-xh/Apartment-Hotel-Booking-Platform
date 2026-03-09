using Booking.Application.Abstractions.CreateProperty;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using MediatR;

namespace Booking.Application.Features.Properties.GetMyProperties;

public sealed class GetMyPropertiesQueryHandler
    : IRequestHandler<GetMyPropertiesQuery, List<GetMyPropertiesResponse>>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyPropertiesQueryHandler(
        IGenericRepository<Property> propertyRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<GetMyPropertiesResponse>> Handle(GetMyPropertiesQuery request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var properties = await _propertyRepository.GetAllAsync(
            p => p.OwnerId == ownerId,
            ct);

        return properties
            .Select(p => new GetMyPropertiesResponse(
                p.Id,
                p.Name,
                p.Description,
                p.PropertyType.ToString(),
                p.MaxGuests,
                p.IsActive,
                p.IsApproved
            ))
            .ToList();
    }
}
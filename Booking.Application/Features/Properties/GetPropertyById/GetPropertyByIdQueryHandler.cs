
using Booking.Application.Abstractions.Properties;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.PropertyAmenities;

using MediatR;

namespace Booking.Application.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdQueryHandler
    : IRequestHandler<GetPropertyByIdQuery, GetPropertyByIdResponse>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IGenericRepository<PropertyAmenity> _propertyAmenityRepository;

    public GetPropertyByIdQueryHandler(
        IPropertyRepository propertyRepository,
        IGenericRepository<PropertyAmenity> propertyAmenityRepository)
    {
        _propertyRepository = propertyRepository;
        _propertyAmenityRepository = propertyAmenityRepository;
    }

    public async Task<GetPropertyByIdResponse> Handle(GetPropertyByIdQuery request, CancellationToken ct)
    {
        var property = await _propertyRepository.GetPropertyByIdWithDetailsAsync(request.PropertyId, ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        var amenities = await _propertyAmenityRepository.GetAllAsync(
            pa => pa.PropertyId == property.Id,
            ct);

        return new GetPropertyByIdResponse(
            property.Id,
            property.Name,
            property.Description,
            property.PropertyType.ToString(),
            property.MaxGuests,
            property.CheckInTime.ToString(),
            property.CheckOutTime.ToString(),
            property.IsActive,
            property.IsApproved,
            new GetPropertyAddressResponse(
                property.Address.Country,
                property.Address.City,
                property.Address.Street,
                property.Address.PostalCode
            ),
            amenities
                .Select(a => a.Amenity.ToString())
                .ToList()
        );
    }
}
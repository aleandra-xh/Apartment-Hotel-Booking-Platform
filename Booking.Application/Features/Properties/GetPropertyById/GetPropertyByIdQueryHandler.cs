
using Booking.Application.Abstractions.Properties;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.PropertyAmenities;
using Booking.Domain.Reviews;
using MediatR;

namespace Booking.Application.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdQueryHandler
    : IRequestHandler<GetPropertyByIdQuery, GetPropertyByIdResponse>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IGenericRepository<PropertyAmenity> _propertyAmenityRepository;
    private readonly IGenericRepository<Review> _reviewRepository;

    public GetPropertyByIdQueryHandler(
        IPropertyRepository propertyRepository,
        IGenericRepository<PropertyAmenity> propertyAmenityRepository,
        IGenericRepository<Review> reviewRepository)
    {
        _propertyRepository = propertyRepository;
        _propertyAmenityRepository = propertyAmenityRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<GetPropertyByIdResponse> Handle(GetPropertyByIdQuery request, CancellationToken ct)
    {
        var property = await _propertyRepository.GetPropertyByIdWithDetailsAsync(request.PropertyId, ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        var amenities = await _propertyAmenityRepository.GetAllAsync(
            pa => pa.PropertyId == property.Id,
            ct);

        var reservationIds = property.Reservations
            .Select(r => r.Id)
            .ToList();

        var reviews = reservationIds.Count == 0
            ? new List<Review>()
            : await _reviewRepository.GetAllAsync(
                r => reservationIds.Contains(r.ReservationId),
                ct);

        var averageRating = reviews.Count == 0
            ? 0
            : Math.Round(reviews.Average(r => r.Rating), 2);

        var reviewsCount = reviews.Count;

        var imagesCount = property.Images.Count;

        return new GetPropertyByIdResponse(
            property.Id,
            property.Name,
            property.Description,
            property.PropertyType.ToString(),
            property.MaxGuests,
            property.PricePerNight,
            property.CleaningFee,
            property.ServiceFee,
            property.TaxPercentage,
            property.AdditionalGuestFeePerNight,
            property.BaseGuestCount,
            property.MinStayNights,
            property.MaxStayNights,
            property.CheckInTime.ToString(),
            property.CheckOutTime.ToString(),
            property.IsActive,
            property.IsApproved,
            property.CreatedAt,
            property.LastModifiedAt,
            averageRating,
            reviewsCount,
            property.Images.Count,

    new GetPropertyAddressResponse(
        property.Address.Country,
        property.Address.City,
        property.Address.Street,
        property.Address.PostalCode
    ),
     amenities.Select(a => a.Amenity.ToString()).ToList()
     );
   }
}
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.PropertyAmenities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Unit>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IGenericRepository<PropertyAmenity> _propertyAmenityRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePropertyCommandHandler(
        IGenericRepository<Property> propertyRepository,
        IGenericRepository<PropertyAmenity> propertyAmenityRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _propertyAmenityRepository = propertyAmenityRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdatePropertyCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to update this property.");

        var duplicateExists = await _propertyRepository.AnyAsync(
            p => p.OwnerId == ownerId &&
                 p.Name == request.Request.Name &&
                 p.Id != request.PropertyId,
            ct);

        if (duplicateExists)
            throw new ConflictException("You already have another property with this name.");

        property.Name = request.Request.Name;
        property.Description = request.Request.Description;
        property.PropertyType = (PropertyType)request.Request.PropertyType;
        property.MaxGuests = request.Request.MaxGuests;
        property.PricePerNight = request.Request.PricePerNight;
        property.CleaningFee = request.Request.CleaningFee;
        property.ServiceFee = request.Request.ServiceFee;
        property.TaxPercentage = request.Request.TaxPercentage;
        property.AdditionalGuestFeePerNight = request.Request.AdditionalGuestFeePerNight;
        property.BaseGuestCount = request.Request.BaseGuestCount;
        property.CheckInTime = TimeSpan.Parse(request.Request.CheckInTime);
        property.CheckOutTime = TimeSpan.Parse(request.Request.CheckOutTime);
        property.LastModifiedAt = DateTime.UtcNow;

        var existingAmenities = await _propertyAmenityRepository.GetAllAsync(
            pa => pa.PropertyId == property.Id,
            ct);

        foreach (var existingAmenity in existingAmenities)
        {
            _propertyAmenityRepository.Remove(existingAmenity);
        }

        var newAmenities = request.Request.Amenities
            .Distinct()
            .Select(a => new PropertyAmenity
            {
                PropertyId = property.Id,
                Amenity = (Amenity)a
            })
            .ToList();

        foreach (var amenity in newAmenities)
        {
            await _propertyAmenityRepository.AddAsync(amenity, ct);
        }

        try
        {
            await _propertyRepository.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            throw new ConflictException("Unable to update property due to duplicate or invalid data.");
        }

        return Unit.Value;
    }
}
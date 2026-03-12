using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Addresses;
using Booking.Domain.Properties;
using Booking.Domain.PropertyAmenities;
using MediatR;

namespace Booking.Application.Features.Properties.CreateProperty;

public sealed class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Guid>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IGenericRepository<Address> _addressRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreatePropertyCommandHandler(
        IGenericRepository<Property> propertyRepository,
        IGenericRepository<Address> addressRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _addressRepository = addressRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreatePropertyCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;
        var alreadyExists = await _propertyRepository.AnyAsync(
            p => p.OwnerId == ownerId && p.Name == request.Request.Name,
            ct);

        if (alreadyExists)
            throw new ConflictException("You already have a property with this name.");

        var address = new Address
        {
            Id = Guid.NewGuid(),
            Country = request.Request.Address.Country,
            City = request.Request.Address.City,
            Street = request.Request.Address.Street,
            PostalCode = request.Request.Address.PostalCode
        };

        await _addressRepository.AddAsync(address, ct);

        var property = new Property
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = request.Request.Name,
            Description = request.Request.Description,
            PropertyType = (PropertyType)request.Request.PropertyType,
            AddressId = address.Id,
            MaxGuests = request.Request.MaxGuests,
            CheckInTime = TimeSpan.Parse(request.Request.CheckInTime),
            CheckOutTime = TimeSpan.Parse(request.Request.CheckOutTime),
            IsActive = true,
            IsApproved = false,
            CreatedAt = DateTime.UtcNow
        };

        property.Amenities = request.Request.Amenities
            .Distinct()
            .Select(a => new PropertyAmenity
            {
                PropertyId = property.Id,
                Amenity = (Amenity)a
            })
            .ToList();

        await _propertyRepository.AddAsync(property, ct);
        await _propertyRepository.SaveChangesAsync(ct);

        return property.Id;
    }
}

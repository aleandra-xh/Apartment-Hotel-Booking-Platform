using Booking.Application.Abstractions.Properties;
using Booking.Application.Abstractions.Reservations;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Reservations;
using MediatR;
using Booking.Application.Abstractions.Security;

namespace Booking.Application.Features.Reservations.CreateReservation;

public sealed class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IGenericRepository<Reservation> _genericReservationRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateReservationCommandHandler(
        IPropertyRepository propertyRepository,
        IReservationRepository reservationRepository,
        IGenericRepository<Reservation> genericReservationRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _reservationRepository = reservationRepository;
        _genericReservationRepository = genericReservationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken ct)
    {
        var guestId = _currentUserService.UserId;

        var property = await _propertyRepository.GetPropertyForReservationAsync(
            request.Request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (!property.IsActive)
            throw new ConflictException("Property is not active.");

        if (!property.IsApproved)
            throw new ConflictException("Property is not approved.");

        if (request.Request.GuestCount > property.MaxGuests)
            throw new ConflictException("Guest count exceeds the property's maximum capacity.");

        if (property.OwnerId == guestId)
            throw new ConflictException("Owners cannot reserve their own property.");

        var hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(
            request.Request.PropertyId,
            request.Request.StartDate,
            request.Request.EndDate,
            ct);

        if (hasOverlap)
            throw new ConflictException("Property is not available for the selected dates.");

        int numberOfNights = (request.Request.EndDate.Date - request.Request.StartDate.Date).Days;

        decimal priceForPeriod = property.PricePerNight * numberOfNights;

        int extraGuests = Math.Max(0, request.Request.GuestCount - property.BaseGuestCount);

        decimal additionalGuestFee = extraGuests * property.AdditionalGuestFeePerNight * numberOfNights;

        decimal cleaningFee = property.CleaningFee;

        decimal serviceFee = property.ServiceFee;

        decimal subtotal = priceForPeriod + additionalGuestFee + cleaningFee + serviceFee;

        decimal taxAmount = subtotal * (property.TaxPercentage / 100m);

        decimal totalPrice = subtotal + taxAmount;

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            PropertyId = property.Id,
            GuestId = guestId,
            StartDate = request.Request.StartDate.Date,
            EndDate = request.Request.EndDate.Date,
            GuestCount = request.Request.GuestCount,

            CleaningFee = cleaningFee,
            AdditionalGuestFee = additionalGuestFee,
            ServiceFee = serviceFee,
            TaxAmount = taxAmount,
            AmenitiesUpCharge = 0,
            PriceForPeriod = priceForPeriod,
            TotalPrice = totalPrice,

            BookingStatus = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _genericReservationRepository.AddAsync(reservation, ct);
        await _genericReservationRepository.SaveChangesAsync(ct);

        return reservation.Id;
    }
}

using Booking.Application.Abstractions.Properties;
using Booking.Application.Abstractions.PropertyBlockedDates;
using Booking.Application.Abstractions.PropertyDiscounts;
using Booking.Application.Abstractions.PropertySeasonalPrices;
using Booking.Application.Abstractions.Reservations;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.CreateReservation;

public sealed class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IGenericRepository<Reservation> _genericReservationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPropertyBlockedDateRepository _blockedDateRepository;
    private readonly IPropertySeasonalPriceRepository _seasonalPriceRepository;
    private readonly IPropertyDiscountRepository _discountRepository;

    public CreateReservationCommandHandler(
        IPropertyRepository propertyRepository,
        IReservationRepository reservationRepository,
        IGenericRepository<Reservation> genericReservationRepository,
        IPropertyBlockedDateRepository blockedDateRepository,
        IPropertySeasonalPriceRepository seasonalPriceRepository,
        IPropertyDiscountRepository discountRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _reservationRepository = reservationRepository;
        _genericReservationRepository = genericReservationRepository;
        _currentUserService = currentUserService;
        _blockedDateRepository = blockedDateRepository;
        _seasonalPriceRepository = seasonalPriceRepository;
        _discountRepository = discountRepository;
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

        int numberOfNights = (request.Request.EndDate.Date - request.Request.StartDate.Date).Days;

        if (numberOfNights < property.MinStayNights)
            throw new ConflictException($"Minimum stay for this property is {property.MinStayNights} nights.");

        if (numberOfNights > property.MaxStayNights)
            throw new ConflictException($"Maximum stay for this property is {property.MaxStayNights} nights.");

        var hasBlockedDates = await _blockedDateRepository.HasOverlappingBlockedDateAsync(
            request.Request.PropertyId,
            request.Request.StartDate,
            request.Request.EndDate,
            ct);

        if (hasBlockedDates)
            throw new ConflictException("Property is blocked for the selected dates.");

        var hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(
            request.Request.PropertyId,
            request.Request.StartDate,
            request.Request.EndDate,
            ct);

        if (hasOverlap)
            throw new ConflictException("Property is not available for the selected dates.");

        var baseOrSeasonalPricePerNight = await _seasonalPriceRepository.GetApplicablePricePerNightAsync(
            property.Id,
            request.Request.StartDate,
            request.Request.EndDate,
            ct) ?? property.PricePerNight;

        var discountPercentage = await _discountRepository.GetApplicableDiscountPercentageAsync(
            property.Id,
            request.Request.StartDate,
            request.Request.EndDate,
            ct) ?? 0m;

        var discountedPricePerNight = baseOrSeasonalPricePerNight * (1 - (discountPercentage / 100m));

        decimal priceForPeriod = discountedPricePerNight * numberOfNights;

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
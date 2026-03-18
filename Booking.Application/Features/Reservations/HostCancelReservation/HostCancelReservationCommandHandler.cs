using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.HostCancelReservation;

public sealed class HostCancelReservationCommandHandler : IRequestHandler<HostCancelReservationCommand, Unit>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public HostCancelReservationCommandHandler(
        IGenericRepository<Reservation> reservationRepository,
        IGenericRepository<Property> propertyRepository,
        ICurrentUserService currentUserService)
    {
        _reservationRepository = reservationRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(HostCancelReservationCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var reservation = await _reservationRepository.FirstOrDefaultAsync(
            r => r.Id == request.ReservationId,
            ct);

        if (reservation is null)
            throw new NotFoundException("Reservation not found.");

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == reservation.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to cancel this reservation.");

        if (reservation.BookingStatus == ReservationStatus.Cancelled)
            throw new ConflictException("Reservation is already cancelled.");

        if (reservation.BookingStatus == ReservationStatus.Completed)
            throw new ConflictException("Completed reservations cannot be cancelled.");

        if (reservation.BookingStatus == ReservationStatus.Rejected)
            throw new ConflictException("Rejected reservations cannot be cancelled.");

        if (reservation.BookingStatus == ReservationStatus.Expired)
            throw new ConflictException("Expired reservations cannot be cancelled.");

        if (reservation.StartDate.Date <= DateTime.UtcNow.Date)
            throw new ConflictException("Reservation cannot be cancelled on or after the start date.");

        var daysBeforeStart = (reservation.StartDate.Date - DateTime.UtcNow.Date).Days;

        decimal refundAmount;
        decimal penaltyAmount;

        if (daysBeforeStart >= 7)
        {
            refundAmount = reservation.TotalPrice;
            penaltyAmount = 0;
        }
        else if (daysBeforeStart >= 2)
        {
            refundAmount = reservation.TotalPrice * 0.5m;
            penaltyAmount = reservation.TotalPrice - refundAmount;
        }
        else
        {
            refundAmount = 0;
            penaltyAmount = reservation.TotalPrice;
        }

        reservation.BookingStatus = ReservationStatus.Cancelled;
        reservation.CancelledOnUtc = DateTime.UtcNow;
        reservation.LastModifiedAt = DateTime.UtcNow;

        await _reservationRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

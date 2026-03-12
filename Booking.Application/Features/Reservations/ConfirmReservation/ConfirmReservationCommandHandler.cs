
using Booking.Application.Abstractions.Properties;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.ConfirmReservation;

public sealed class ConfirmReservationCommandHandler : IRequestHandler<ConfirmReservationCommand, Unit>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public ConfirmReservationCommandHandler(
        IGenericRepository<Reservation> reservationRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService)
    {
        _reservationRepository = reservationRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ConfirmReservationCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var reservation = await _reservationRepository.FirstOrDefaultAsync(
            r => r.Id == request.ReservationId,
            ct);

        if (reservation is null)
            throw new NotFoundException("Reservation not found.");

        var property = await _propertyRepository.GetPropertyForReservationAsync(
            reservation.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to confirm this reservation.");

        if (reservation.BookingStatus != ReservationStatus.Pending)
            throw new ConflictException("This booking cannot be accepted because it is no longer pending.");

        reservation.BookingStatus = ReservationStatus.Confirmed;
        reservation.ConfirmedOnUtc = DateTime.UtcNow;
        reservation.LastModifiedAt = DateTime.UtcNow;

        await _reservationRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
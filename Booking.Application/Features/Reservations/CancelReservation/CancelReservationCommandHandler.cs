
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.CancelReservation;

public sealed class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, Unit>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly ICurrentUserService _currentUserService;

    public CancelReservationCommandHandler(
        IGenericRepository<Reservation> reservationRepository,
        ICurrentUserService currentUserService)
    {
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(CancelReservationCommand request, CancellationToken ct)
    {
        var guestId = _currentUserService.UserId;

        var reservation = await _reservationRepository.FirstOrDefaultAsync(
            r => r.Id == request.ReservationId,
            ct);

        if (reservation is null)
            throw new NotFoundException("Reservation not found.");

        if (reservation.GuestId != guestId)
            throw new UnauthorizedException("You are not allowed to cancel this reservation.");

        if (reservation.BookingStatus == ReservationStatus.Cancelled)
            throw new ConflictException("Reservation is already cancelled.");

        if (reservation.BookingStatus == ReservationStatus.Completed)
            throw new ConflictException("Completed reservations cannot be cancelled.");

        if (reservation.StartDate.Date <= DateTime.UtcNow.Date)
            throw new ConflictException("Reservation cannot be cancelled on or after the start date.");

        reservation.BookingStatus = ReservationStatus.Cancelled;
        reservation.CancelledOnUtc = DateTime.UtcNow;
        reservation.LastModifiedAt = DateTime.UtcNow;

        await _reservationRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
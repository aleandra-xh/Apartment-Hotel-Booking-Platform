
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Abstractions.Properties;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.CompleteReservation;

public sealed class CompleteReservationCommandHandler : IRequestHandler<CompleteReservationCommand, Unit>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;

    public CompleteReservationCommandHandler(
        IGenericRepository<Reservation> reservationRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService,
        INotificationService notificationService)
    {
        _reservationRepository = reservationRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(CompleteReservationCommand request, CancellationToken ct)
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
            throw new UnauthorizedException("You are not allowed to complete this reservation.");

        if (reservation.BookingStatus == ReservationStatus.Completed)
            throw new ConflictException("This booking has already been completed.");

        if (reservation.BookingStatus != ReservationStatus.Confirmed)
            throw new ConflictException("Only confirmed bookings can be completed.");

        if (reservation.EndDate.Date >= DateTime.UtcNow.Date)
            throw new ConflictException("This booking cannot be completed before the stay has ended.");

        reservation.BookingStatus = ReservationStatus.Completed;
        reservation.CompletedOnUtc = DateTime.UtcNow;
        reservation.LastModifiedAt = DateTime.UtcNow;

        await _reservationRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            reservation.GuestId,
            "Booking completed",
            $"Your stay for property '{property.Name}' has been completed. You can now leave a review.",
            NotificationType.BookingCompleted,
            ct);

        return Unit.Value;
    }
}
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.Properties;
using Booking.Domain.Reservations;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Features.Reservations.HostCancelReservation;

public sealed class HostCancelReservationCommandHandler : IRequestHandler<HostCancelReservationCommand, Unit>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IEmailService _emailService;

    public HostCancelReservationCommandHandler(
        IGenericRepository<Reservation> reservationRepository,
        IGenericRepository<Property> propertyRepository,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        IGenericRepository<User> userRepository,
        IEmailService emailService)
    {
        _reservationRepository = reservationRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _userRepository = userRepository;
        _emailService = emailService;
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
        reservation.RefundAmount = refundAmount;
        reservation.PenaltyAmount = penaltyAmount;
        reservation.CancelledOnUtc = DateTime.UtcNow;
        reservation.LastModifiedAt = DateTime.UtcNow;

        await _reservationRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            reservation.GuestId,
            "Booking cancelled",
            $"Your reservation for property '{property.Name}' has been cancelled by the host.",
            NotificationType.BookingCancelled,
            ct);

        var guest = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == reservation.GuestId,
            ct);

        if (guest is not null && !string.IsNullOrWhiteSpace(guest.Email))
        {
            await _emailService.SendAsync(
                new EmailMessage(
                    guest.Email,
                    "Booking cancelled",
                    $"Your reservation for property '{property.Name}' has been cancelled by the host."
                ),
                ct);
        }

        return Unit.Value;
    }
}
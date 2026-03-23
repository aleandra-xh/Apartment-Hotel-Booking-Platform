using Booking.Application.Abstractions.Notifications;
using Booking.Application.Abstractions.Properties;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.Reservations;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Features.Reservations.CancelReservation;

public sealed class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, Unit>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IEmailService _emailService;

    public CancelReservationCommandHandler(
        IGenericRepository<Reservation> reservationRepository,
        IPropertyRepository propertyRepository,
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

        var property = await _propertyRepository.GetPropertyForReservationAsync(
            reservation.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

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
            property.OwnerId,
            "Booking cancelled",
            $"A reservation for your property '{property.Name}' has been cancelled by the guest.",
            NotificationType.BookingCancelled,
            ct);

        var owner = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == property.OwnerId,
            ct);

        if (owner is not null && !string.IsNullOrWhiteSpace(owner.Email))
        {
            await _emailService.SendAsync(
                new EmailMessage(
                    owner.Email,
                    "Booking cancelled",
                    $"A reservation for your property '{property.Name}' has been cancelled by the guest."
                ),
                ct);
        }

        return Unit.Value;
    }
}
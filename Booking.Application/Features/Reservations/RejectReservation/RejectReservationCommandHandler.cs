
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Abstractions.Properties;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.Reservations;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Features.Reservations.RejectReservation;

public sealed class RejectReservationCommandHandler : IRequestHandler<RejectReservationCommand, Unit>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IEmailService _emailService;

    public RejectReservationCommandHandler(
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

    public async Task<Unit> Handle(RejectReservationCommand request, CancellationToken ct)
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
            throw new UnauthorizedException("You are not allowed to reject this reservation.");

        if (reservation.BookingStatus == ReservationStatus.Rejected)
            throw new ConflictException("This booking has already been rejected.");

        if (reservation.BookingStatus != ReservationStatus.Pending)
            throw new ConflictException("This booking cannot be rejected because it has already been processed.");

        reservation.BookingStatus = ReservationStatus.Rejected;
        reservation.RejectedOnUtc = DateTime.UtcNow;
        reservation.LastModifiedAt = DateTime.UtcNow;

        await _reservationRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            reservation.GuestId,
            "Booking rejected",
            $"Your reservation for property '{property.Name}' has been rejected.",
            NotificationType.BookingRejected,
            ct);

        var guest = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == reservation.GuestId,
            ct);

        if (guest is not null && !string.IsNullOrWhiteSpace(guest.Email))
        {
            await _emailService.SendAsync(
                new EmailMessage(
                    guest.Email,
                    "Booking rejected",
                    $"Your reservation for property '{property.Name}' has been rejected."
                ),
                ct);
        }

        return Unit.Value;
    }
}

using Booking.Application.Abstractions.Notifications;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.Properties;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Features.Properties.SuspendProperty;

public sealed class SuspendPropertyCommandHandler : IRequestHandler<SuspendPropertyCommand, Unit>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;

    public SuspendPropertyCommandHandler(
        IGenericRepository<Property> propertyRepository,
        IGenericRepository<User> userRepository,
        INotificationService notificationService,
        IEmailService emailService)
    {
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(SuspendPropertyCommand request, CancellationToken ct)
    {
        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (!property.IsActive)
            throw new ConflictException("Property is already suspended.");

        property.IsActive = false;
        property.LastModifiedAt = DateTime.UtcNow;

        await _propertyRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            property.OwnerId,
            "Property suspended",
            $"Your property '{property.Name}' has been suspended by an administrator.",
            NotificationType.PropertySuspended,
            ct);

        var owner = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == property.OwnerId,
            ct);

        if (owner is not null && !string.IsNullOrWhiteSpace(owner.Email))
        {
            await _emailService.SendAsync(
                new EmailMessage(
                    owner.Email,
                    "Property suspended",
                    $"Your property '{property.Name}' has been suspended by an administrator."
                ),
                ct);
        }

        return Unit.Value;
    }
}
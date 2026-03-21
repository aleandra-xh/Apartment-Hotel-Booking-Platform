
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.Properties;
using MediatR;

namespace Booking.Application.Features.Properties.RejectProperty;

public sealed class RejectPropertyCommandHandler : IRequestHandler<RejectPropertyCommand, Unit>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly INotificationService _notificationService;

    public RejectPropertyCommandHandler(
        IGenericRepository<Property> propertyRepository,
        INotificationService notificationService)
    {
        _propertyRepository = propertyRepository;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(RejectPropertyCommand request, CancellationToken ct)
    {
        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (!property.IsActive)
            throw new ConflictException("Inactive property cannot be rejected.");

        property.IsApproved = false;
        property.LastModifiedAt = DateTime.UtcNow;

        await _propertyRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            property.OwnerId,
            "Property rejected",
            $"Your property '{property.Name}' has been rejected.",
            NotificationType.PropertyRejected,
            ct);

        return Unit.Value;
    }
}
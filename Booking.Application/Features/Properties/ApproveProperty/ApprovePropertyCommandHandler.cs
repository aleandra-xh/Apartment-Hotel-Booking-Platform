
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.Properties;
using MediatR;

namespace Booking.Application.Features.Properties.ApproveProperty;

public sealed class ApprovePropertyCommandHandler : IRequestHandler<ApprovePropertyCommand, Unit>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly INotificationService _notificationService;

    public ApprovePropertyCommandHandler(
        IGenericRepository<Property> propertyRepository,
        INotificationService notificationService)
    {
        _propertyRepository = propertyRepository;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(ApprovePropertyCommand request, CancellationToken ct)
    {
        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.IsApproved)
            throw new ConflictException("Property is already approved.");

        property.IsApproved = true;
        property.LastModifiedAt = DateTime.UtcNow;

        await _propertyRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            property.OwnerId,
            "Property approved",
            $"Your property '{property.Name}' has been approved.",
            NotificationType.PropertyApproved,
            ct);

        return Unit.Value;
    }
}
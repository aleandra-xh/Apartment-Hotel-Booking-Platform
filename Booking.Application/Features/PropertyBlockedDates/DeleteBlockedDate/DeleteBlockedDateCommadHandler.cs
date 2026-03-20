
using Booking.Application.Abstractions.PropertyBlockedDates;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.PropertyBlockedDates;
using MediatR;

namespace Booking.Application.Features.PropertyBlockedDates.DeleteBlockedDate;

public sealed class DeleteBlockedDateCommandHandler : IRequestHandler<DeleteBlockedDateCommand, Unit>
{
    private readonly IPropertyBlockedDateRepository _blockedDateRepository;
    private readonly IGenericRepository<PropertyBlockedDate> _genericBlockedDateRepository;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteBlockedDateCommandHandler(
        IPropertyBlockedDateRepository blockedDateRepository,
        IGenericRepository<PropertyBlockedDate> genericBlockedDateRepository,
        IGenericRepository<Property> propertyRepository,
        ICurrentUserService currentUserService)
    {
        _blockedDateRepository = blockedDateRepository;
        _genericBlockedDateRepository = genericBlockedDateRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteBlockedDateCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var blockedDate = await _blockedDateRepository.GetByIdAsync(request.BlockedDateId, ct);

        if (blockedDate is null)
            throw new NotFoundException("Blocked date entry not found.");

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == blockedDate.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to delete blocked dates for this property.");

        _genericBlockedDateRepository.Remove(blockedDate);
        await _genericBlockedDateRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
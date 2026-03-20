
using Booking.Application.Abstractions.PropertyBlockedDates;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.PropertyBlockedDates;
using MediatR;

namespace Booking.Application.Features.PropertyBlockedDates.BlockPropertyDates;

public sealed class BlockPropertyDatesCommandHandler : IRequestHandler<BlockPropertyDatesCommand, Guid>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IGenericRepository<PropertyBlockedDate> _genericBlockedDateRepository;
    private readonly IPropertyBlockedDateRepository _blockedDateRepository;
    private readonly ICurrentUserService _currentUserService;

    public BlockPropertyDatesCommandHandler(
        IGenericRepository<Property> propertyRepository,
        IGenericRepository<PropertyBlockedDate> genericBlockedDateRepository,
        IPropertyBlockedDateRepository blockedDateRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _genericBlockedDateRepository = genericBlockedDateRepository;
        _blockedDateRepository = blockedDateRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(BlockPropertyDatesCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.Request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to block dates for this property.");

        var hasOverlap = await _blockedDateRepository.HasOverlappingBlockedDateAsync(
            request.Request.PropertyId,
            request.Request.StartDate,
            request.Request.EndDate,
            ct);

        if (hasOverlap)
            throw new ConflictException("This property already has blocked dates that overlap with the selected range.");

        var blockedDate = new PropertyBlockedDate
        {
            Id = Guid.NewGuid(),
            PropertyId = request.Request.PropertyId,
            StartDate = request.Request.StartDate.Date,
            EndDate = request.Request.EndDate.Date,
            Reason = request.Request.Reason,
            CreatedAt = DateTime.UtcNow
        };

        await _genericBlockedDateRepository.AddAsync(blockedDate, ct);
        await _genericBlockedDateRepository.SaveChangesAsync(ct);

        return blockedDate.Id;
    }
}

using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Application.Abstractions.CreateProperty;
using MediatR;


namespace Booking.Application.Features.Properties.DeleteProperty;

public sealed class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand>
{
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeletePropertyCommandHandler(
        IGenericRepository<Property> propertyRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeletePropertyCommand request, CancellationToken ct)
    {
        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != _currentUserService.UserId)
            throw new UnauthorizedException("You are not allowed to delete this property.");

        _propertyRepository.Remove(property);

        await _propertyRepository.SaveChangesAsync(ct);
    }
}
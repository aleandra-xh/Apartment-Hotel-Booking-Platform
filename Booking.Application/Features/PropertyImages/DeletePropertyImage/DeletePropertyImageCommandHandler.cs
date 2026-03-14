
using Booking.Application.Abstractions.Properties;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.PropertyImages;
using MediatR;

namespace Booking.Application.Features.PropertyImages.DeletePropertyImage;
public sealed class DeletePropertyImageCommandHandler : IRequestHandler<DeletePropertyImageCommand, Unit>
{
    private readonly IGenericRepository<PropertyImage> _propertyImageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeletePropertyImageCommandHandler(
        IGenericRepository<PropertyImage> propertyImageRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService)
    {
        _propertyImageRepository = propertyImageRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeletePropertyImageCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var image = await _propertyImageRepository.FirstOrDefaultAsync(
            pi => pi.Id == request.ImageId,
            ct);

        if (image is null)
            throw new NotFoundException("Property image not found.");

        var property = await _propertyRepository.GetPropertyForReservationAsync(
            image.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to delete images from this property.");

        var propertyImages = await _propertyImageRepository.GetAllAsync(
            pi => pi.PropertyId == image.PropertyId,
            ct);

        if (propertyImages.Count <= 3)
            throw new ConflictException("A property must have at least 3 images.");

        _propertyImageRepository.Remove(image);
        await _propertyImageRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
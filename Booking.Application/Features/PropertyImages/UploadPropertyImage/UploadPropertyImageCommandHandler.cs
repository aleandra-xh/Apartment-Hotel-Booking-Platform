
using Booking.Application.Abstractions.Properties;
using Booking.Application.Abstractions.PropertyImages;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.PropertyImages;
using MediatR;
using System.Security.Cryptography;

namespace Booking.Application.Features.PropertyImages.UploadPropertyImage;

public sealed class UploadPropertyImageCommandHandler : IRequestHandler<UploadPropertyImageCommand, List<Guid>>
{
    private readonly IGenericRepository<PropertyImage> _propertyImageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPropertyImageRepository _propertyImageReadRepository;

    public UploadPropertyImageCommandHandler(
        IGenericRepository<PropertyImage> propertyImageRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService,
        IPropertyImageRepository propertyImageReadRepository)
    {
        _propertyImageRepository = propertyImageRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
        _propertyImageReadRepository = propertyImageReadRepository;
    }

    public async Task<List<Guid>> Handle(UploadPropertyImageCommand request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var property = await _propertyRepository.GetPropertyForReservationAsync(
            request.Request.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedException("You are not allowed to upload images for this property.");

        var existingImagesCount = await _propertyImageReadRepository.CountByPropertyIdAsync(property.Id, ct);
        var newImagesCount = request.Request.Images.Count;

        if (existingImagesCount + newImagesCount > 10)
            throw new ConflictException("A property cannot have more than 10 images in total.");

        var requestImageHashes = request.Request.Images
            .Select(i => ComputeSha256(i.ImageData))
            .ToList();

        if (requestImageHashes.Count != requestImageHashes.Distinct().Count())
            throw new ConflictException("Duplicate images are not allowed.");

        var imageIds = new List<Guid>();

        foreach (var image in request.Request.Images)
        {
            var imageHash = ComputeSha256(image.ImageData);

            var exists = await _propertyImageReadRepository.ExistsByHashAsync(
                property.Id,
                imageHash,
                ct);

            if (exists)
                throw new ConflictException("One or more uploaded images already exist for this property.");

            var propertyImage = new PropertyImage
            {
                Id = Guid.NewGuid(),
                PropertyId = property.Id,
                ImageData = image.ImageData,
                FileName = image.FileName,
                ContentType = image.ContentType,
                ImageHash = imageHash,
                CreatedAt = DateTime.UtcNow
            };

            await _propertyImageRepository.AddAsync(propertyImage, ct);
            imageIds.Add(propertyImage.Id);
        }

        await _propertyImageRepository.SaveChangesAsync(ct);

        return imageIds;
    }

    private static string ComputeSha256(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(data);
        return Convert.ToHexString(hashBytes);
    }
}

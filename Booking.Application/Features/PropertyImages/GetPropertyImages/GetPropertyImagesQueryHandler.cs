
using Booking.Application.Generics.Interfaces;
using Booking.Domain.PropertyImages;
using MediatR;

namespace Booking.Application.Features.PropertyImages.GetPropertyImages;

public sealed class GetPropertyImagesQueryHandler
    : IRequestHandler<GetPropertyImagesQuery, List<GetPropertyImagesResponse>>
{
    private readonly IGenericRepository<PropertyImage> _propertyImageRepository;

    public GetPropertyImagesQueryHandler(IGenericRepository<PropertyImage> propertyImageRepository)
    {
        _propertyImageRepository = propertyImageRepository;
    }

    public async Task<List<GetPropertyImagesResponse>> Handle(GetPropertyImagesQuery request, CancellationToken ct)
    {
        var images = await _propertyImageRepository.GetAllAsync(
            pi => pi.PropertyId == request.PropertyId,
            ct);

        return images
            .OrderByDescending(pi => pi.CreatedAt)
            .Select(pi => new GetPropertyImagesResponse(
                pi.Id,
                pi.FileName,
                pi.ContentType,
                Convert.ToBase64String(pi.ImageData),
                pi.CreatedAt
            ))
            .ToList();
    }
}

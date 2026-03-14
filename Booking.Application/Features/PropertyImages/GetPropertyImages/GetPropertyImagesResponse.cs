
namespace Booking.Application.Features.PropertyImages.GetPropertyImages;

public sealed record GetPropertyImagesResponse(
    Guid Id,
    string FileName,
    string ContentType,
    string Base64Image,
    DateTime CreatedAt
);
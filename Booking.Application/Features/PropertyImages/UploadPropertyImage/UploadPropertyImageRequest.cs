
namespace Booking.Application.Features.PropertyImages.UploadPropertyImage;

public sealed record UploadPropertyImageItem(
    byte[] ImageData,
    string FileName,
    string ContentType
);

public sealed record UploadPropertyImageRequest(
    Guid PropertyId,
    List<UploadPropertyImageItem> Images
);
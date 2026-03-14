
namespace Booking.Application.Features.Users.UploadUserProfileImage;

public sealed record UploadUserProfileImageRequest(
    string FileName,
    string ContentType,
    byte[] ImageData
);
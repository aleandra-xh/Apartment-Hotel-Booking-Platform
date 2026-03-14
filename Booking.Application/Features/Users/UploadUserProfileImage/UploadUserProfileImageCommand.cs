
using MediatR;

namespace Booking.Application.Features.Users.UploadUserProfileImage;
public sealed record UploadUserProfileImageCommand(
    UploadUserProfileImageRequest Request
) : IRequest<string>;

using MediatR;

namespace Booking.Application.Features.PropertyImages.UploadPropertyImage;

public sealed record UploadPropertyImageCommand(UploadPropertyImageRequest Request) : IRequest<List<Guid>>;
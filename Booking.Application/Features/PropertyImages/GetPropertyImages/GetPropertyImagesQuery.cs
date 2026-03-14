
using MediatR;

namespace Booking.Application.Features.PropertyImages.GetPropertyImages;

public sealed record GetPropertyImagesQuery(Guid PropertyId) : IRequest<List<GetPropertyImagesResponse>>;
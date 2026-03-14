
using MediatR;

namespace Booking.Application.Features.PropertyImages.DeletePropertyImage;

public sealed record DeletePropertyImageCommand(Guid ImageId) : IRequest<Unit>;
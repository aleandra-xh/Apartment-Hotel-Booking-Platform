using MediatR;

namespace Booking.Application.Features.Properties.GetPropertyById;

public sealed record GetPropertyByIdQuery(Guid PropertyId) : IRequest<GetPropertyByIdResponse>;

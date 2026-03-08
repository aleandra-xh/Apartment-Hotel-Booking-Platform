using MediatR;

namespace Booking.Application.Features.Properties.GetMyProperties;

public sealed record GetMyPropertiesQuery : IRequest<List<GetMyPropertiesResponse>>;

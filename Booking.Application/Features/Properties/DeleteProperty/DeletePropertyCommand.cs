using MediatR;

namespace Booking.Application.Features.Properties.DeleteProperty;

public sealed record DeletePropertyCommand(Guid PropertyId) : IRequest;

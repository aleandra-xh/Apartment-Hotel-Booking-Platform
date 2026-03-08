using MediatR;

namespace Booking.Application.Features.BecomeOwner;

public sealed record BecomeOwnerCommand(BecomeOwnerRequest Request) : IRequest<Unit>;

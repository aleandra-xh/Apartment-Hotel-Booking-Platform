using MediatR;

namespace Booking.Application.Features.Users.BecomeOwner;

public sealed record BecomeOwnerCommand(BecomeOwnerRequest Request) : IRequest<Unit>;

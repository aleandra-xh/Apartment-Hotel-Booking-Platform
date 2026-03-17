
using MediatR;

namespace Booking.Application.Features.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    ChangePasswordRequest Request
) : IRequest<Unit>;
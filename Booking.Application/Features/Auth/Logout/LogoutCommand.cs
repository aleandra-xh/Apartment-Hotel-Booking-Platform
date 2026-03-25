
using MediatR;

namespace Booking.Application.Features.Auth.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Unit>;
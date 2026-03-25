
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Booking.API.Hubs;

[Authorize]
public sealed class NotificationHub : Hub
{
}
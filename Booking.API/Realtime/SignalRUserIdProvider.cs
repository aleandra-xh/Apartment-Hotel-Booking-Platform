
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Booking.API.Realtime;

public sealed class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
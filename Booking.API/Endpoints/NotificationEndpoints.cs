using Booking.Application.Features.Notifications.GetMyNotifications;
using Booking.Application.Features.Notifications.MarkAllNotificationsAsRead;
using Booking.Application.Features.Notifications.MarkNotificationAsRead;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Booking.API.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        //---Get My Notifications---
        app.MapGet("/v1/notifications/my", [Authorize] async (
            ISender sender,
            int page = 1,
            int pageSize = 10) =>
        {
            var result = await sender.Send(new GetMyNotificationsQuery(page, pageSize));
            return Results.Ok(result);
        })
        .WithName("GetMyNotifications");

        //---Mark Notification As Read---
        app.MapPut("/v1/notifications/{id:guid}/read", [Authorize] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new MarkNotificationAsReadCommand(id));
            return Results.Ok("Notification marked as read.");
        })
        .WithName("MarkNotificationAsRead");

        //---Mark All Notifications As Read---
        app.MapPut("/v1/notifications/read-all", [Authorize] async (
            ISender sender) =>
        {
            await sender.Send(new MarkAllNotificationsAsReadCommand());
            return Results.Ok("All notifications marked as read.");
        })
        .WithName("MarkAllNotificationsAsRead");
    }
}
using Booking.Application.Features.Properties.ApproveProperty;
using Booking.Application.Features.Properties.RejectProperty;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Booking.API.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        //---Approve Property---
        app.MapPut("/v1/properties/{id:guid}/approve", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new ApprovePropertyCommand(id));
            return Results.Ok("Property approved successfully.");
        })
        .WithName("ApproveProperty");


        //---Reject Property---
        app.MapPut("/v1/properties/{id:guid}/reject", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new RejectPropertyCommand(id));
            return Results.Ok("Property rejected successfully.");
        })
        .WithName("RejectProperty");
    }
}
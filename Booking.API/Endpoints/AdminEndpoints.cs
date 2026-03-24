using Booking.Application.Features.Admin.RemoveOwnerRole;
using Booking.Application.Features.Properties.ApproveProperty;
using Booking.Application.Features.Properties.RejectProperty;
using Booking.Application.Features.Properties.SuspendProperty;
using Booking.Application.Features.Reservations.GetAllReservationsForAdmin;
using Booking.Application.Features.Users.ApproveOwnerRequest;
using Booking.Application.Features.Users.DeleteUser;
using Booking.Application.Features.Users.GetAllUsersForAdmin;
using Booking.Application.Features.Users.GetPendingOwnerRequests;
using Booking.Application.Features.Users.RejectOwnerRequest;
using Booking.Application.Features.Users.SuspendUser;
using Booking.Domain.Reservations;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Booking.API.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {

        //---Get All Reservations For Admin---
        app.MapGet("/v1/admin/reservations", [Authorize(Roles = "Admin")] async (
            ReservationStatus? status,
            ISender sender,
            int page = 1,
            int pageSize = 10) =>
        {
            var result = await sender.Send(new GetAllReservationsForAdminQuery(status, page, pageSize));
            return Results.Ok(result);
        })
        .WithName("GetAllReservationsForAdmin");

        //---Get All Users For Admin---
        app.MapGet("/v1/admin/users", [Authorize(Roles = "Admin")] async (
            ISender sender,
            int page = 1,
            int pageSize = 10
            ) =>
        {
            var result = await sender.Send(new GetAllUsersForAdminQuery(page, pageSize));
            return Results.Ok(result);
        })
        .WithName("GetAllUsersForAdmin");


        //---Suspend User---
        app.MapPut("/v1/admin/users/{id:guid}/suspend", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new SuspendUserCommand(id));
            return Results.Ok("User suspended successfully.");
        })
        .WithName("SuspendUser");


        //---Delete User--- 
        app.MapDelete("/v1/admin/users/{id:guid}", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new DeleteUserCommand(id));
            return Results.Ok("User deleted successfully.");
        })
        .WithName("DeleteUser");

        //---Suspend Property---
        app.MapPut("/v1/admin/properties/{id:guid}/suspend", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new SuspendPropertyCommand(id));
            return Results.Ok("Property suspended successfully.");
        })
        .WithName("SuspendProperty");

        //---Approve Property---
        app.MapPut("/v1/admin/properties/{id:guid}/approve", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new ApprovePropertyCommand(id));
            return Results.Ok("Property approved successfully.");
        })
        .WithName("ApproveProperty");


        //---Reject Property---
        app.MapPut("/v1/admin/properties/{id:guid}/reject", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new RejectPropertyCommand(id));
            return Results.Ok("Property rejected successfully.");
        })
        .WithName("RejectProperty");

        //---Approve Owner Request---
        app.MapPut("/v1/admin/users/{id:guid}/approve-owner-request", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new ApproveOwnerRequestCommand(id));
            return Results.Ok("Owner request approved successfully.");
        })
        .WithName("ApproveOwnerRequest");

        //---Reject Owner Request---
        app.MapPut("/v1/admin/users/{id:guid}/reject-owner-request", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new RejectOwnerRequestCommand(id));
            return Results.Ok("Owner request rejected successfully.");
        })
        .WithName("RejectOwnerRequest");

        //---Get Pending Owner Requests---
        app.MapGet("/v1/admin/owner-requests/pending", [Authorize(Roles = "Admin")] async (
            ISender sender,
            int page = 1,
            int pageSize = 10) =>
        {
            var result = await sender.Send(new GetPendingOwnerRequestsQuery(page, pageSize));
            return Results.Ok(result);
        })
        .WithName("GetPendingOwnerRequests");

        //---Remove Owner Role---
        app.MapPut("/v1/admin/users/{id:guid}/remove-owner-role", [Authorize(Roles = "Admin")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new RemoveOwnerRoleCommand(id));
            return Results.Ok("Owner role removed successfully.");
        })
        .WithName("RemoveOwnerRole");
    }
}
using Booking.Application.Features.Reservations.CancelReservation;
using Booking.Application.Features.Reservations.CompleteReservation;
using Booking.Application.Features.Reservations.ConfirmReservation;
using Booking.Application.Features.Reservations.CreateReservation;
using Booking.Application.Features.Reservations.GetMyReservations;
using Booking.Application.Features.Reservations.GetOwnerReservations;
using Booking.Application.Features.Reservations.RejectReservation;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Booking.API.Endpoints;

public static class ReservationEndpoint

{
    public static void MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        //---Create Reservation---
        app.MapPost("/v1/reservations/create", [Authorize] async (CreateReservationCommand command, 
            ISender sender) =>
        {
            var reservationId = await sender.Send(command);
            return Results.Created($"/v1/reservations/{reservationId}", null);
        })
        .WithName("CreateReservation");


        //---Get My Reservations---
        app.MapGet("/v1/reservations/my", [Authorize] async (ISender sender) =>
        {
            var result = await sender.Send(new GetMyReservationsQuery());
            return Results.Ok(result);
        })
        .WithName("GetMyReservations");


        //---Get Owner Reservation
        app.MapGet("/v1/reservations/owner", [Authorize(Roles = "Owner")] async (ISender sender) =>
        {
            var result = await sender.Send(new GetOwnerReservationsQuery());
            return Results.Ok(result);
        })
        .WithName("GetOwnerReservations");


        //---Cancel Reservation---
        app.MapPut("/v1/reservations/cancel/{id:guid}", [Authorize] async (Guid id,
            ISender sender) =>
        {
            await sender.Send(new CancelReservationCommand(id));
            return Results.Ok("Reservation cancelled successfully.");
        })
        .WithName("CancelReservation");


        //---Confirm Reservation
        app.MapPut("/v1/reservations/confirm/{id:guid}", [Authorize(Roles = "Owner")] async (Guid id, ISender sender) =>
        {
            await sender.Send(new ConfirmReservationCommand(id));
            return Results.Ok("Reservation confirmed successfully.");
        })
        .WithName("ConfirmReservation");


        //--Reject Reservation
        app.MapPut("/v1/reservations/reject/{id:guid}", [Authorize(Roles = "Owner")] async (Guid id, ISender sender) =>
        {
            await sender.Send(new RejectReservationCommand(id));
            return Results.Ok("Booking rejected successfully.");
        })
        .WithName("RejectReservation");


        //---Complete Reservation---
        app.MapPut("/v1/reservations/complete/{id:guid}", [Authorize(Roles = "Owner")] async (Guid id, ISender sender) =>
        {
            await sender.Send(new CompleteReservationCommand(id));
            return Results.Ok("Booking completed successfully.");
        })
        .WithName("CompleteReservation");
    }
}
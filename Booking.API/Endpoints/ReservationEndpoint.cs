using Booking.Application.Features.Reservations.CreateReservation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Booking.Application.Features.Reservations.GetMyReservations;

namespace Booking.API.Endpoints;

public static class ReservationEndpoint

{
    public static void MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        //---Create Reservation---
        app.MapPost("/v1/reservations/create", [Authorize] async (CreateReservationCommand command, ISender sender) =>
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
    }
}
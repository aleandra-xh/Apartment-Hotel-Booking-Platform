using Booking.Application.Features.Reservations.CreateReservation;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Booking.API.Endpoints;

public static class ReservationEndpoint

{
    public static void MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        //Create Reservation
        app.MapPost("/v1/reservations/create", [Authorize] async (CreateReservationCommand command, ISender sender) =>
        {
            var reservationId = await sender.Send(command);
            return Results.Created($"/v1/reservations/{reservationId}", null);
        })
        .WithName("CreateReservation");
    }
}
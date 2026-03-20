using Booking.Application.Features.Properties.CreateProperty;
using Booking.Application.Features.Properties.DeleteProperty;
using Booking.Application.Features.Properties.GetMyProperties;
using Booking.Application.Features.Properties.GetPropertyById;
using Booking.Application.Features.Properties.SearchProperties;
using Booking.Application.Features.Properties.UpdateProperty;
using Booking.Application.Features.PropertyBlockedDates.BlockPropertyDates;
using Booking.Application.Features.PropertyBlockedDates.GetPropertyBlockedDates;
using Booking.Application.Features.PropertyBlockedDates.DeleteBlockedDate;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Booking.Api;

public static class PropertyEndpoint
{
    public static void MapPropertyEndpoints(this IEndpointRouteBuilder app)
    {
        //---Create Properties---

        app.MapPost("/v1/properties/create/", [Authorize(Roles = "Owner")] async (CreatePropertyCommand command, ISender sender) =>
        {
            var propertyId = await sender.Send(command);
            return Results.Created($"/v1/properties/create/{propertyId}", null);
        })
        .WithName("CreateProperty");

        //---Get My Properties---

        app.MapGet("/v1/properties/my/properties", [Authorize(Roles = "Owner")] async (ISender sender) =>
        {
            var result = await sender.Send(new GetMyPropertiesQuery());
            return Results.Ok(result);
        })
        .WithName("GetMyProperties");

        //---Update Property---

        app.MapPut("/v1/properties/update/{id:guid}", [Authorize(Roles = "Owner")] async (
            Guid id,
            UpdatePropertyRequest request,
            ISender sender) =>
        {
            await sender.Send(new UpdatePropertyCommand(id, request));
            return Results.Ok("Property updated successfully.");
        })
        .WithName("UpdateProperty");

        //---Delete Property---
        app.MapDelete("/v1/properties/delete/{id:guid}",
        [Authorize(Roles = "Owner")] async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeletePropertyCommand(id));
            return Results.NoContent();
        })
        .WithName("DeleteProperty");

        //---Get Property By Id---
        app.MapGet("/v1/properties/getbyid/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetPropertyByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetPropertyById");

        //---Search Property---
        app.MapPost("/v1/properties/search", async (SearchPropertiesRequest request, ISender sender) =>
        {
            var result = await sender.Send(new SearchPropertiesQuery(request));
            return Results.Ok(result);
        })
        .WithName("SearchProperties");


        //---Block Property Dates---
        app.MapPost("/v1/properties/blocked-dates", [Authorize(Roles = "Owner")] async (
    BlockPropertyDatesCommand command,
    ISender sender) =>
        {
            var blockedDateId = await sender.Send(command);
            return Results.Created($"/v1/properties/blocked-dates/{blockedDateId}", null);
        })
            .WithName("BlockPropertyDates");


        //---Get Property Blocked Dates---

        app.MapGet("/v1/properties/{propertyId:guid}/blocked-dates", [Authorize(Roles = "Owner")] async (
            Guid propertyId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetPropertyBlockedDatesQuery(propertyId));
            return Results.Ok(result);
        })
        .WithName("GetPropertyBlockedDates");


        //---Delete Blocked Date---

        app.MapDelete("/v1/properties/blocked-dates/{id:guid}", [Authorize(Roles = "Owner")] async (
            Guid id,
            ISender sender) =>
        {
            await sender.Send(new DeleteBlockedDateCommand(id));
            return Results.Ok("Blocked dates deleted successfully.");
        })
        .WithName("DeleteBlockedDate");
    }
}   
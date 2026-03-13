using Booking.Application.Features.Reviews.CreateReview;
using Booking.Application.Features.Reviews.GetPropertyReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Booking.API.Endpoints;

public static class ReviewEndpoint
{
    public static void MapReviewEndpoints(this IEndpointRouteBuilder app)
    {
        //---Create Review---
        app.MapPost("/v1/reviews/create", [Authorize] async (CreateReviewCommand command, ISender sender) =>
        {
            var reviewId = await sender.Send(command);
            return Results.Created($"/v1/reviews/{reviewId}", null);
        })
        .WithName("CreateReview");


        //---Get Property Reviews---
        app.MapGet("/v1/reviews/property/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetPropertyReviewsQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetPropertyReviews");

    }
}
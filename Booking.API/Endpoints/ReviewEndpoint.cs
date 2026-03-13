using Booking.Application.Features.Reviews.CreateReview;
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
    }
}
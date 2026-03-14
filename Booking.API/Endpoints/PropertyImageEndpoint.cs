using Booking.Application.Features.PropertyImages.DeletePropertyImage;
using Booking.Application.Features.PropertyImages.GetPropertyImages;
using Booking.Application.Features.PropertyImages.UploadPropertyImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Booking.API.Endpoints;

public static class PropertyImageEndpoint
{
    public static void MapPropertyImageEndpoints(this IEndpointRouteBuilder app)
    {
        //---Upload Property Images
        app.MapPost("/v1/properties/images/{id:guid}", [Authorize(Roles = "Owner")] async (
            Guid id,
            HttpRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            if (!request.HasFormContentType)
                return Results.BadRequest("Request must be sent as form-data.");

            var form = await request.ReadFormAsync(ct);
            var files = form.Files;

            if (files is null || files.Count == 0)
                return Results.BadRequest("At least 3 image files are required.");

            var images = new List<UploadPropertyImageItem>();

            foreach (var file in files)
            {
                await using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, ct);

                images.Add(new UploadPropertyImageItem(
                    memoryStream.ToArray(),
                    file.FileName,
                    file.ContentType
                ));
            }

            var command = new UploadPropertyImageCommand(
                new UploadPropertyImageRequest(
                    id,
                    images
                ));

            var imageIds = await sender.Send(command, ct);

            return Results.Created($"/v1/properties/images/{id}", imageIds);
        })
        .WithName("UploadPropertyImages");

        //---Get Property Images---
        app.MapGet("/v1/properties/get/images/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetPropertyImagesQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetPropertyImages");

        //---Delete Property Images---
        app.MapDelete("/v1/properties/delete/images/{id:guid}", [Authorize(Roles = "Owner")] async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeletePropertyImageCommand(id));
            return Results.Ok("Property image deleted successfully.");
        })
        .WithName("DeletePropertyImage");
    }
}
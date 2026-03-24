using Booking.Application.Features.Auth.Login;
using Booking.Application.Features.Users.BecomeOwner;
using Booking.Application.Features.Users.ChangePassword;
using Booking.Application.Features.Users.DeleteUserProfileImage;
using Booking.Application.Features.Users.GetMyProfile;
using Booking.Application.Features.Users.GetUserProfileImage;
using Booking.Application.Features.Users.RegisterUser;
using Booking.Application.Features.Users.UpdateUserProfile;
using Booking.Application.Features.Users.UploadUserProfileImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Booking.Api;

public static class UserEndpoint
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // ---Users---
        app.MapPost("/v1/users/register", async (RegisterUserCommand command, ISender sender) =>
        {
            var userId = await sender.Send(command);
            return Results.Created($"/v1/users/{userId}", null);
        })
        .WithName("RegisterUser");

        // ---Auth---
        app.MapPost("/v1/users/auth/login", async (LoginCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .WithName("LoginUser");

        // ---Become-Owner---

        app.MapPost("/v1/users/become-owner", [Authorize] async (BecomeOwnerCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.Ok("Owner request submitted successfully. Your verification is pending admin approval.");
        })
        .WithName("BecomeOwner");

        //---Upload User Profile Image---
        app.MapPost("/v1/users/profile-image", [Authorize] async (
        HttpRequest request,
        ISender sender,
        CancellationToken ct) =>
        {
            if (!request.HasFormContentType)
                return Results.BadRequest("Request must be sent as form-data.");

            var form = await request.ReadFormAsync(ct);
            var image = form.Files["image"];

            if (image is null || image.Length == 0)
                return Results.BadRequest("Image file is required.");

            await using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream, ct);

            var command = new UploadUserProfileImageCommand(
                new UploadUserProfileImageRequest(
                    image.FileName,
                    image.ContentType,
                    memoryStream.ToArray()
                ));

            var profileImageUrl = await sender.Send(command, ct);

            return Results.Ok(new { profileImageUrl });
        })
        .WithName("UploadUserProfileImage");

        //--- Get User Profile Image---
        app.MapGet("/v1/users/profile-image/get", [Authorize] async (ISender sender) =>
        {
            var result = await sender.Send(new GetUserProfileImageQuery());
            return Results.Ok(result);
        })
        .WithName("GetUserProfileImage");

        //---Delete User Profile Image---
        app.MapDelete("/v1/users/profile-image/delete", [Authorize] async (ISender sender) =>
        {
            await sender.Send(new DeleteUserProfileImageCommand());
            return Results.Ok("Profile image deleted successfully.");
        })
        .WithName("DeleteUserProfileImage");

        //---Update User Profile---
        app.MapPut("/v1/users/update/profile", [Authorize] async (UpdateUserProfileCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.Ok("Profile updated successfully.");
        })
        .WithName("UpdateUserProfile");

        //---Change Password---
        app.MapPut("/v1/users/change/password", [Authorize] async (ChangePasswordCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.Ok("Password changed successfully.");
        })
        .WithName("ChangePassword");

        //---Get My Profile---
        app.MapGet("/v1/users/me", [Authorize] async (ISender sender) =>
        {
            var result = await sender.Send(new GetMyProfileQuery());
            return Results.Ok(result);
        })
        .WithName("GetMyProfile");
    }

}
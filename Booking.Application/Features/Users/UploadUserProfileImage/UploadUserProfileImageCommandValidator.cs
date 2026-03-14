
using FluentValidation;

namespace Booking.Application.Features.Users.UploadUserProfileImage;

public sealed class UploadUserProfileImageCommandValidator : AbstractValidator<UploadUserProfileImageCommand>
{
    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public UploadUserProfileImageCommandValidator()
    {
        RuleFor(x => x.Request.ImageData)
            .NotNull()
            .WithMessage("Image data is required.")
            .Must(data => data.Length > 0)
            .WithMessage("Image cannot be empty.")
            .Must(data => data.Length <= 5 * 1024 * 1024)
            .WithMessage("Image size cannot exceed 5 MB.");

        RuleFor(x => x.Request.FileName)
            .NotEmpty()
            .WithMessage("File name is required.")
            .MaximumLength(255)
            .WithMessage("File name cannot exceed 255 characters.");

        RuleFor(x => x.Request.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required.")
            .Must(contentType => AllowedContentTypes.Contains(contentType))
            .WithMessage("Only JPEG, PNG, and WEBP images are allowed.");
    }
}

using FluentValidation;

namespace Booking.Application.Features.PropertyImages.UploadPropertyImage;

public sealed class UploadPropertyImageCommandValidator : AbstractValidator<UploadPropertyImageCommand>
{
    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public UploadPropertyImageCommandValidator()
    {
        RuleFor(x => x.Request.PropertyId)
            .NotEmpty()
            .WithMessage("Property id is required.");

        RuleFor(x => x.Request.Images)
            .NotNull()
            .WithMessage("Images are required.")
            .Must(images => images.Count >= 3)
            .WithMessage("A property must have at least 3 images.")
            .Must(images => images.Count <= 10)
            .WithMessage("A property cannot have more than 10 images.");

        RuleForEach(x => x.Request.Images).ChildRules(image =>
        {
            image.RuleFor(i => i.ImageData)
                .NotNull()
                .WithMessage("Image data is required.")
                .Must(data => data.Length > 0)
                .WithMessage("Image cannot be empty.")
                .Must(data => data.Length <= 5 * 1024 * 1024)
                .WithMessage("Each image size cannot exceed 5 MB.");

            image.RuleFor(i => i.FileName)
                .NotEmpty()
                .WithMessage("File name is required.")
                .MaximumLength(255)
                .WithMessage("File name cannot exceed 255 characters.");

            image.RuleFor(i => i.ContentType)
                .NotEmpty()
                .WithMessage("Content type is required.")
                .Must(contentType => AllowedContentTypes.Contains(contentType))
                .WithMessage("Only JPEG, PNG, and WEBP images are allowed.");
        });
    }
}
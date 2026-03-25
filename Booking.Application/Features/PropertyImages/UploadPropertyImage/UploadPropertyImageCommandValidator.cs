
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

    private static readonly string[] AllowedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    public UploadPropertyImageCommandValidator()
    {
        RuleFor(x => x.Request.PropertyId)
            .NotEmpty()
            .WithMessage("Property id is required.");

        RuleFor(x => x.Request.Images)
            .NotNull()
            .WithMessage("Images are required.")
            .Must(images => images.Count >= 1)
            .WithMessage("At least one image is required.")
            .Must(images => images.Count <= 10)
            .WithMessage("You cannot upload more than 10 images in a single request.");

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
                .WithMessage("File name cannot exceed 255 characters.")
                .Must(fileName =>
                {
                    var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
                    return !string.IsNullOrWhiteSpace(extension) && AllowedExtensions.Contains(extension);
                })
                .WithMessage("Only .jpg, .jpeg, .png and .webp files are allowed.");

            image.RuleFor(i => i.ContentType)
                .NotEmpty()
                .WithMessage("Content type is required.")
                .Must(contentType => AllowedContentTypes.Contains(contentType.ToLowerInvariant()))
                .WithMessage("Only JPEG, PNG, and WEBP images are allowed.");
        });
    }
}
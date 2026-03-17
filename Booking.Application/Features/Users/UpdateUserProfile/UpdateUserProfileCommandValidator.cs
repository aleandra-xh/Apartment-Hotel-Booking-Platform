
using FluentValidation;

namespace Booking.Application.Features.Users.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.Request)
            .Must(r =>
                !string.IsNullOrWhiteSpace(r.FirstName) ||
                !string.IsNullOrWhiteSpace(r.LastName) ||
                !string.IsNullOrWhiteSpace(r.PhoneNumber))
            .WithMessage("At least one field must be provided for update.");

        When(x => x.Request.FirstName is not null, () =>
        {
            RuleFor(x => x.Request.FirstName)
                .NotEmpty()
                .WithMessage("First name cannot be empty.")
                .MaximumLength(100)
                .WithMessage("First name cannot exceed 100 characters.");
        });

        When(x => x.Request.LastName is not null, () =>
        {
            RuleFor(x => x.Request.LastName)
                .NotEmpty()
                .WithMessage("Last name cannot be empty.")
                .MaximumLength(100)
                .WithMessage("Last name cannot exceed 100 characters.");
        });

        When(x => x.Request.PhoneNumber is not null, () =>
        {
            RuleFor(x => x.Request.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number cannot be empty.")
                .MaximumLength(20)
                .WithMessage("Phone number cannot exceed 20 characters.");
        });
    }
}
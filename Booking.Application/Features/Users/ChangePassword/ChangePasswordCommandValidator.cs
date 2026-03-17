
using FluentValidation;

namespace Booking.Application.Features.Users.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Request.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.");

        RuleFor(x => x.Request.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters long.");

        RuleFor(x => x.Request.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("Confirm new password is required.")
            .Equal(x => x.Request.NewPassword)
            .WithMessage("Confirm new password must match the new password.");

        RuleFor(x => x.Request.NewPassword)
            .NotEqual(x => x.Request.CurrentPassword)
            .WithMessage("New password must be different from the current password.");
    }
}
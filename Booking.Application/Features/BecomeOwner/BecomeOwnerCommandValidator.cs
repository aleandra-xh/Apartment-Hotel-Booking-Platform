using FluentValidation;

namespace Booking.Application.Features.BecomeOwner;

public sealed class BecomeOwnerCommandValidator : AbstractValidator<BecomeOwnerCommand>
{
    public BecomeOwnerCommandValidator()
    {
        RuleFor(x => x.Request.IdentityCardNumber)
            .NotEmpty().WithMessage("Identity card number is required.")
            .MinimumLength(6)
            .MaximumLength(20)
            .Matches("^[A-Z0-9]+$")
            .WithMessage("Identity card number must contain only uppercase letters and digits.");

        RuleFor(x => x.Request.CreditCard)
            .NotEmpty().WithMessage("Credit card is required.")
            .Matches(@"^\d{16}$")
            .WithMessage("Credit card must contain exactly 16 digits.");

        RuleFor(x => x.Request.BusinessName)
            .NotEmpty().WithMessage("Business name is required.")
            .MinimumLength(3)
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9\s\-\&]+$")
            .WithMessage("Business name contains invalid characters.");
    }
}
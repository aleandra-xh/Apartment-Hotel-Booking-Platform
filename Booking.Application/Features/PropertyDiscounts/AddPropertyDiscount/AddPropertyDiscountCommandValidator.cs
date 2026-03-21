
using FluentValidation;

namespace Booking.Application.Features.PropertyDiscounts.AddPropertyDiscount;

public sealed class AddPropertyDiscountCommandValidator : AbstractValidator<AddPropertyDiscountCommand>
{
    public AddPropertyDiscountCommandValidator()
    {
        RuleFor(x => x.Request.PropertyId)
            .NotEmpty()
            .WithMessage("Property id is required.");

        RuleFor(x => x.Request.StartDate)
            .Must(d => d.Date >= DateTime.UtcNow.Date)
            .WithMessage("Discount start date cannot be in the past.");

        RuleFor(x => x.Request.EndDate)
            .Must((request, endDate) => endDate.Date > request.Request.StartDate.Date)
            .WithMessage("Discount end date must be greater than start date.");

        RuleFor(x => x.Request.Percentage)
            .GreaterThan(0)
            .WithMessage("Discount percentage must be greater than 0.");

        RuleFor(x => x.Request.Percentage)
            .LessThanOrEqualTo(100)
            .WithMessage("Discount percentage cannot exceed 100.");

        RuleFor(x => x.Request.Label)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Label))
            .WithMessage("Label cannot exceed 200 characters.");
    }
}
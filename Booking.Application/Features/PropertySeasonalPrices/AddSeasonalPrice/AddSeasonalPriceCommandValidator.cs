
using FluentValidation;

namespace Booking.Application.Features.PropertySeasonalPrices.AddSeasonalPrice;

public sealed class AddSeasonalPriceCommandValidator : AbstractValidator<AddSeasonalPriceCommand>
{
    public AddSeasonalPriceCommandValidator()
    {
        RuleFor(x => x.Request.PropertyId)
            .NotEmpty()
            .WithMessage("Property id is required.");

        RuleFor(x => x.Request.StartDate)
            .Must(d => d.Date >= DateTime.UtcNow.Date)
            .WithMessage("Seasonal price start date cannot be in the past.");

        RuleFor(x => x.Request.EndDate)
            .Must((request, endDate) => endDate.Date > request.Request.StartDate.Date)
            .WithMessage("Seasonal price end date must be greater than start date.");

        RuleFor(x => x.Request.PricePerNight)
            .GreaterThan(0)
            .WithMessage("Seasonal price per night must be greater than 0.");

        RuleFor(x => x.Request.Label)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Label))
            .WithMessage("Label cannot exceed 200 characters.");
    }
}
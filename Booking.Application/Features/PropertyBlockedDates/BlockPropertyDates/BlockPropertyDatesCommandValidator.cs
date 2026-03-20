
using FluentValidation;

namespace Booking.Application.Features.PropertyBlockedDates.BlockPropertyDates;
public sealed class BlockPropertyDatesCommandValidator : AbstractValidator<BlockPropertyDatesCommand>
{
    public BlockPropertyDatesCommandValidator()
    {
        RuleFor(x => x.Request.PropertyId)
            .NotEmpty()
            .WithMessage("Property id is required.");

        RuleFor(x => x.Request.StartDate)
            .Must(d => d.Date >= DateTime.UtcNow.Date)
            .WithMessage("Blocked start date cannot be in the past.");

        RuleFor(x => x.Request.EndDate)
            .Must((request, endDate) => endDate.Date > request.Request.StartDate.Date)
            .WithMessage("Blocked end date must be greater than start date.");

        RuleFor(x => x.Request.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Reason))
            .WithMessage("Reason cannot exceed 500 characters.");
    }
}
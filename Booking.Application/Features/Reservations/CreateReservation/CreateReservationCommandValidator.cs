

using FluentValidation;

namespace Booking.Application.Features.Reservations.CreateReservation;

public sealed class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.Request.PropertyId)
            .NotEmpty().WithMessage("Property id is required.");

        RuleFor(x => x.Request.GuestCount)
            .GreaterThan(0).WithMessage("Guest count must be greater than 0.")
            .LessThanOrEqualTo(50).WithMessage("Guest count cannot exceed 50.");

        RuleFor(x => x.Request.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.Request.EndDate)
            .NotEmpty().WithMessage("End date is required.");

        RuleFor(x => x.Request)
            .Must(x => x.StartDate.Date < x.EndDate.Date)
            .WithMessage("End date must be later than start date.");

        RuleFor(x => x.Request.StartDate)
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Start date cannot be in the past.");
    }
}
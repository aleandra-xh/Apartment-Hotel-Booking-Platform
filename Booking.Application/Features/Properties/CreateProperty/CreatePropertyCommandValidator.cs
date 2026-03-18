using Booking.Domain.Properties;
using FluentValidation;

namespace Booking.Application.Features.Properties.CreateProperty;

public sealed class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Property name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Request.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000);

        RuleFor(x => x.Request.MaxGuests)
            .GreaterThan(0).WithMessage("Max guests must be greater than 0.");

        RuleFor(x => x.Request.PropertyType)
            .Must(value => Enum.IsDefined(typeof(PropertyType), value))
            .WithMessage("Invalid property type.");

        RuleFor(x => x.Request.CheckInTime)
            .NotEmpty().WithMessage("Check-in time is required.");

        RuleFor(x => x.Request.CheckOutTime)
            .NotEmpty().WithMessage("Check-out time is required.");

        RuleFor(x => x.Request.CheckInTime)
            .Must(BeValidTimeSpan)
            .WithMessage("Invalid check-in time format. Use HH:mm or HH:mm:ss.");

        RuleFor(x => x.Request.CheckOutTime)
            .Must(BeValidTimeSpan)
            .WithMessage("Invalid check-out time format. Use HH:mm or HH:mm:ss.");

        RuleFor(x => x.Request.Amenities)
            .NotNull().WithMessage("Amenities are required.")
            .NotEmpty().WithMessage("At least one amenity is required.");

        RuleForEach(x => x.Request.Amenities)
            .Must(value => Enum.IsDefined(typeof(Amenity), value))
            .WithMessage("Invalid amenity value.");

        RuleFor(x => x.Request.Address)
            .NotNull().WithMessage("Address is required.");

        RuleFor(x => x.Request.Address.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100);

        RuleFor(x => x.Request.Address.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100);

        RuleFor(x => x.Request.Address.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(150);

        RuleFor(x => x.Request.Address.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.")
            .MaximumLength(20);

        RuleFor(x => x.Request.PricePerNight)
            .GreaterThan(0)
            .WithMessage("Price per night must be greater than 0.");

        RuleFor(x => x.Request.CleaningFee)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Cleaning fee cannot be negative.");

        RuleFor(x => x.Request.ServiceFee)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Service fee cannot be negative.");

        RuleFor(x => x.Request.TaxPercentage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tax percentage cannot be negative.")
            .LessThanOrEqualTo(100)
            .WithMessage("Tax percentage cannot exceed 100.");

        RuleFor(x => x.Request.AdditionalGuestFeePerNight)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Additional guest fee per night cannot be negative.");

        RuleFor(x => x.Request.BaseGuestCount)
            .GreaterThan(0)
            .WithMessage("Base guest count must be greater than 0.");

        RuleFor(x => x.Request.BaseGuestCount)
            .LessThanOrEqualTo(x => x.Request.MaxGuests)
            .WithMessage("Base guest count cannot exceed max guests.");
        RuleFor(x => x.Request.MinStayNights)
            .GreaterThan(0)
            .WithMessage("Minimum stay nights must be greater than 0.");

        RuleFor(x => x.Request.MaxStayNights)
            .GreaterThan(0)
            .WithMessage("Maximum stay nights must be greater than 0.");

        RuleFor(x => x.Request.MaxStayNights)
            .GreaterThanOrEqualTo(x => x.Request.MinStayNights)
            .WithMessage("Maximum stay nights must be greater than or equal to minimum stay nights.");
    }

    private static bool BeValidTimeSpan(string value)
        => TimeSpan.TryParse(value, out _);
}
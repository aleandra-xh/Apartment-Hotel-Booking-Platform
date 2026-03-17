using Booking.Domain.Properties;
using FluentValidation;

namespace Booking.Application.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
{
    public UpdatePropertyCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty().WithMessage("Property id is required.");

        RuleFor(x => x.Request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Property name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Property name cannot be empty or whitespace.")
            .MinimumLength(3).WithMessage("Property name must be at least 3 characters long.")
            .MaximumLength(100).WithMessage("Property name cannot exceed 100 characters.")
            .Must(name => name.Trim() == name)
            .WithMessage("Property name cannot start or end with spaces.");

        RuleFor(x => x.Request.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Description is required.")
            .Must(description => !string.IsNullOrWhiteSpace(description))
            .WithMessage("Description cannot be empty or whitespace.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Request.MaxGuests)
            .GreaterThan(0).WithMessage("Max guests must be greater than 0.")
            .LessThanOrEqualTo(50).WithMessage("Max guests cannot exceed 50.");

        RuleFor(x => x.Request.PropertyType)
            .Must(value => Enum.IsDefined(typeof(PropertyType), value))
            .WithMessage("Invalid property type.");

        RuleFor(x => x.Request.CheckInTime)
            .NotEmpty().WithMessage("Check-in time is required.")
            .Must(BeValidTimeSpan)
            .WithMessage("Invalid check-in time format. Use HH:mm or HH:mm:ss.");

        RuleFor(x => x.Request.CheckOutTime)
            .NotEmpty().WithMessage("Check-out time is required.")
            .Must(BeValidTimeSpan)
            .WithMessage("Invalid check-out time format. Use HH:mm or HH:mm:ss.");

        RuleFor(x => x.Request)
            .Must(request => HaveValidCheckInAndCheckOut(request.CheckInTime, request.CheckOutTime))
            .WithMessage("Check-out time must be later than check-in time.");

        RuleFor(x => x.Request.Amenities)
            .NotNull().WithMessage("Amenities are required.")
            .NotEmpty().WithMessage("At least one amenity is required.")
            .Must(amenities => amenities.Distinct().Count() == amenities.Count)
            .WithMessage("Amenities cannot contain duplicate values.");

        RuleForEach(x => x.Request.Amenities)
            .Must(value => Enum.IsDefined(typeof(Amenity), value))
            .WithMessage("Invalid amenity value.");

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
    }

    private static bool BeValidTimeSpan(string value)
        => TimeSpan.TryParse(value, out _);

    private static bool HaveValidCheckInAndCheckOut(string checkInTime, string checkOutTime)
    {
        if (!TimeSpan.TryParse(checkInTime, out var checkIn))
            return false;

        if (!TimeSpan.TryParse(checkOutTime, out var checkOut))
            return false;

        return checkOut > checkIn;
    }
}
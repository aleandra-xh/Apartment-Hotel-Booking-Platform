using Booking.Domain.Properties;
using FluentValidation;

namespace Booking.Application.Features.Properties.SearchProperties;

public sealed class SearchPropertiesQueryValidator : AbstractValidator<SearchPropertiesQuery>
{
    public SearchPropertiesQueryValidator()
    {
        RuleFor(x => x.Request.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.Request.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");

        RuleFor(x => x.Request.PropertyType)
            .Must(value => value is null || Enum.IsDefined(typeof(PropertyType), value.Value))
            .WithMessage("Invalid property type.");

        RuleFor(x => x.Request.MaxGuests)
            .GreaterThan(0)
            .When(x => x.Request.MaxGuests.HasValue)
            .WithMessage("Max guests must be greater than 0.");

        RuleFor(x => x.Request.City)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.City));

        RuleFor(x => x.Request.StartDate)
       .Must(startDate => !startDate.HasValue || startDate.Value.Date >= DateTime.UtcNow.Date)
       .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.Request.EndDate)
            .Must((request, endDate) =>
                !request.Request.StartDate.HasValue ||
                !endDate.HasValue ||
                endDate.Value.Date > request.Request.StartDate.Value.Date)
            .WithMessage("End date must be greater than start date.");

        RuleFor(x => x.Request.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Request.MinPrice.HasValue)
            .WithMessage("Minimum price cannot be negative.");

        RuleFor(x => x.Request.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Request.MaxPrice.HasValue)
            .WithMessage("Maximum price cannot be negative.");

        RuleFor(x => x.Request.MaxPrice)
            .Must((request, maxPrice) =>
                !request.Request.MinPrice.HasValue ||
                !maxPrice.HasValue ||
                maxPrice.Value >= request.Request.MinPrice.Value)
            .WithMessage("Maximum price must be greater than or equal to minimum price.");

        RuleFor(x => x.Request.AmenityIds)
            .Must(ids => ids == null || ids.Count == 0 || ids.All(id => id > 0))
            .WithMessage("Amenity ids must contain only positive values.");
    }
}
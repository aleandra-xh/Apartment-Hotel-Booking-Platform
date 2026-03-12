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
    }
}
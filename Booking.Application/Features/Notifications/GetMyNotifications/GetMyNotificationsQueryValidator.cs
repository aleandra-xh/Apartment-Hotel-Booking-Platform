
using FluentValidation;

namespace Booking.Application.Features.Notifications.GetMyNotifications;

public sealed class GetMyNotificationsQueryValidator : AbstractValidator<GetMyNotificationsQuery>
{
    public GetMyNotificationsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0.");

        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize cannot exceed 100.");
    }
}
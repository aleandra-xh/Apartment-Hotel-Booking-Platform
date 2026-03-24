
using FluentValidation;

namespace Booking.Application.Features.Reservations.GetAllReservationsForAdmin;

public sealed class GetAllReservationsForAdminQueryValidator : AbstractValidator<GetAllReservationsForAdminQuery>
{
    public GetAllReservationsForAdminQueryValidator()
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
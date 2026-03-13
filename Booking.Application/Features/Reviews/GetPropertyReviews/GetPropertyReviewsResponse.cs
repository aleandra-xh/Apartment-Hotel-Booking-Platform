
namespace Booking.Application.Features.Reviews.GetPropertyReviews;

public sealed record GetPropertyReviewsResponse(
    Guid Id,
    Guid ReservationId,
    Guid GuestId,
    int Rating,
    string Comment,
    DateTime CreatedAt
);
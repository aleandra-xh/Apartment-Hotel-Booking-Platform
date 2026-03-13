
namespace Booking.Application.Features.Reviews.CreateReview;

public sealed record CreateReviewRequest(
    Guid ReservationId,
    int Rating,
    string Comment
);

namespace Booking.Application.Features.Reviews.GetPropertyAverageRating;

public sealed record GetPropertyAverageRatingResponse(
    Guid PropertyId,
    double AverageRating,
    int TotalReviews
);
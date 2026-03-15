
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Reservations;
using Booking.Domain.Reviews;
using MediatR;

namespace Booking.Application.Features.Reviews.GetPropertyAverageRating;
public sealed class GetPropertyAverageRatingQueryHandler
    : IRequestHandler<GetPropertyAverageRatingQuery, GetPropertyAverageRatingResponse>
{
    private readonly IGenericRepository<Review> _reviewRepository;
    private readonly IGenericRepository<Reservation> _reservationRepository;

    public GetPropertyAverageRatingQueryHandler(
        IGenericRepository<Review> reviewRepository,
        IGenericRepository<Reservation> reservationRepository)
    {
        _reviewRepository = reviewRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<GetPropertyAverageRatingResponse> Handle(GetPropertyAverageRatingQuery request, CancellationToken ct)
    {
        var reservations = await _reservationRepository.GetAllAsync(
            r => r.PropertyId == request.PropertyId,
            ct);

        var reservationIds = reservations
            .Select(r => r.Id)
            .ToHashSet();

        var reviews = await _reviewRepository.GetAllAsync(
            r => reservationIds.Contains(r.ReservationId),
            ct);

        var totalReviews = reviews.Count;

        var averageRating = totalReviews == 0
            ? 0
            : reviews.Average(r => r.Rating);

        return new GetPropertyAverageRatingResponse(
            request.PropertyId,
            Math.Round(averageRating, 2),
            totalReviews
        );
    }
}
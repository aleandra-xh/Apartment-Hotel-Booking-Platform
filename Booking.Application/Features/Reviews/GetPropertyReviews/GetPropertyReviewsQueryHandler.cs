
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Reservations;
using Booking.Domain.Reviews;
using MediatR;

namespace Booking.Application.Features.Reviews.GetPropertyReviews;

public sealed class GetPropertyReviewsQueryHandler
    : IRequestHandler<GetPropertyReviewsQuery, List<GetPropertyReviewsResponse>>
{
    private readonly IGenericRepository<Review> _reviewRepository;
    private readonly IGenericRepository<Reservation> _reservationRepository;

    public GetPropertyReviewsQueryHandler(
        IGenericRepository<Review> reviewRepository,
        IGenericRepository<Reservation> reservationRepository)
    {
        _reviewRepository = reviewRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<List<GetPropertyReviewsResponse>> Handle(GetPropertyReviewsQuery request, CancellationToken ct)
    {
        var reviews = await _reviewRepository.GetAllAsync(r => true, ct);
        var reservations = await _reservationRepository.GetAllAsync(
            r => r.PropertyId == request.PropertyId,
            ct);

        var reservationIds = reservations
            .Select(r => r.Id)
            .ToHashSet();

        return reviews
            .Where(r => reservationIds.Contains(r.ReservationId))
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new GetPropertyReviewsResponse(
                r.Id,
                r.ReservationId,
                r.GuestId,
                r.Rating,
                r.Comment,
                r.CreatedAt
            ))
            .ToList();
    }
}
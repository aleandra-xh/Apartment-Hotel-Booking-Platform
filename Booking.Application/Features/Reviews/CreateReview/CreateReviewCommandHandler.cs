
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Reservations;
using Booking.Domain.Reviews;

using MediatR;

namespace Booking.Application.Features.Reviews.CreateReview;

public sealed class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Guid>
{
    private readonly IGenericRepository<Review> _reviewRepository;
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateReviewCommandHandler(
        IGenericRepository<Review> reviewRepository,
        IGenericRepository<Reservation> reservationRepository,
        ICurrentUserService currentUserService)
    {
        _reviewRepository = reviewRepository;
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        var guestId = _currentUserService.UserId;

        var reservation = await _reservationRepository.FirstOrDefaultAsync(
            r => r.Id == request.Request.ReservationId,
            ct);

        if (reservation is null)
            throw new NotFoundException("Reservation not found.");

        if (reservation.GuestId != guestId)
            throw new UnauthorizedException("You are not allowed to review this reservation.");

        if (reservation.BookingStatus != ReservationStatus.Completed)
            throw new ConflictException("You can only review completed bookings.");

        var existingReview = await _reviewRepository.AnyAsync(
            r => r.ReservationId == request.Request.ReservationId,
            ct);

        if (existingReview)
            throw new ConflictException("A review already exists for this reservation.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            ReservationId = reservation.Id,
            GuestId = guestId,
            Rating = request.Request.Rating,
            Comment = request.Request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review, ct);
        await _reviewRepository.SaveChangesAsync(ct);

        return review.Id;
    }
}
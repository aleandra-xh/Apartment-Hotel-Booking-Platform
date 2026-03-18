

using Booking.Application.Abstractions.Security;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.GetMyReservations;

public sealed class GetMyReservationsQueryHandler
    : IRequestHandler<GetMyReservationsQuery, List<GetMyReservationsResponse>>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyReservationsQueryHandler(
        IGenericRepository<Reservation> reservationRepository,
        ICurrentUserService currentUserService)
    {
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<GetMyReservationsResponse>> Handle(GetMyReservationsQuery request, CancellationToken ct)
    {
        var guestId = _currentUserService.UserId;

        var reservations = await _reservationRepository.GetAllAsync(
            r => r.GuestId == guestId,
            ct);

        if (request.Status.HasValue)
        {
            reservations = reservations
                .Where(r => r.BookingStatus == request.Status.Value)
                .ToList();
        }

        if (request.Status.HasValue)
        {
            reservations = reservations
                .Where(r => r.BookingStatus == request.Status.Value)
                .ToList();
        }
        else
        {
            if (request.IsPast.HasValue)
            {
                reservations = request.IsPast.Value
                    ? reservations.Where(r => r.BookingStatus == ReservationStatus.Completed).ToList()
                    : reservations.Where(r =>
                        r.BookingStatus == ReservationStatus.Pending ||
                        r.BookingStatus == ReservationStatus.Confirmed).ToList();
            }
            else
            {
                reservations = reservations.Where(r =>
                    r.BookingStatus == ReservationStatus.Pending ||
                    r.BookingStatus == ReservationStatus.Confirmed ||
                    r.BookingStatus == ReservationStatus.Completed).ToList();
            }
        }

        return reservations
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new GetMyReservationsResponse(
                r.Id,
                r.PropertyId,
                r.StartDate,
                r.EndDate,
                r.GuestCount,
                r.TotalPrice,
                r.BookingStatus,
                r.CreatedAt
            ))
            .ToList();
    }
}
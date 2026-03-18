
using Booking.Application.Abstractions.Security;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.GetOwnerReservations;
public sealed class GetOwnerReservationsQueryHandler
    : IRequestHandler<GetOwnerReservationsQuery, List<GetOwnerReservationsResponse>>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetOwnerReservationsQueryHandler(
        IGenericRepository<Reservation> reservationRepository,
        IGenericRepository<Property> propertyRepository,
        ICurrentUserService currentUserService)
    {
        _reservationRepository = reservationRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<GetOwnerReservationsResponse>> Handle(GetOwnerReservationsQuery request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;

        var ownerProperties = await _propertyRepository.GetAllAsync(
            p => p.OwnerId == ownerId,
            ct);

        var ownerPropertyIds = ownerProperties
            .Select(p => p.Id)
            .ToList();

        var reservations = await _reservationRepository.GetAllAsync(
            r => ownerPropertyIds.Contains(r.PropertyId),
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
            .Select(r => new GetOwnerReservationsResponse(
                r.Id,
                r.PropertyId,
                r.GuestId,
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

using Booking.Application.Abstractions.Queries;
using Booking.Application.Abstractions.Security;
using MediatR;

namespace Booking.Application.Features.Reservations.GetMyReservations;

public sealed class GetMyReservationsQueryHandler
    : IRequestHandler<GetMyReservationsQuery, GetMyReservationsResult>
{
    private readonly IReservationQueryService _reservationQueryService;
    private readonly ICurrentUserService _currentUserService;

    public GetMyReservationsQueryHandler(
        IReservationQueryService reservationQueryService,
        ICurrentUserService currentUserService)
    {
        _reservationQueryService = reservationQueryService;
        _currentUserService = currentUserService;
    }

    public async Task<GetMyReservationsResult> Handle(GetMyReservationsQuery request, CancellationToken ct)
    {
        var guestId = _currentUserService.UserId;
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        return await _reservationQueryService.GetMyReservationsAsync(
            guestId,
            request.Status,
            request.IsPast,
            page,
            pageSize,
            ct);
    }
}
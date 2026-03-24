using Booking.Application.Abstractions.Queries;
using Booking.Application.Abstractions.Security;
using MediatR;

namespace Booking.Application.Features.Reservations.GetOwnerReservations;

public sealed class GetOwnerReservationsQueryHandler
    : IRequestHandler<GetOwnerReservationsQuery, GetOwnerReservationsResult>
{
    private readonly IReservationQueryService _reservationQueryService;
    private readonly ICurrentUserService _currentUserService;

    public GetOwnerReservationsQueryHandler(
        IReservationQueryService reservationQueryService,
        ICurrentUserService currentUserService)
    {
        _reservationQueryService = reservationQueryService;
        _currentUserService = currentUserService;
    }

    public async Task<GetOwnerReservationsResult> Handle(GetOwnerReservationsQuery request, CancellationToken ct)
    {
        var ownerId = _currentUserService.UserId;
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        return await _reservationQueryService.GetOwnerReservationsAsync(
            ownerId,
            request.Status,
            request.IsPast,
            page,
            pageSize,
            ct);
    }
}
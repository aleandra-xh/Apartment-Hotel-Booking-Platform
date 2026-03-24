
using Booking.Application.Abstractions.Queries;
using MediatR;

namespace Booking.Application.Features.Reservations.GetAllReservationsForAdmin;

public sealed class GetAllReservationsForAdminQueryHandler
    : IRequestHandler<GetAllReservationsForAdminQuery, GetAllReservationsForAdminResult>
{
    private readonly IAdminQueryService _adminQueryService;

    public GetAllReservationsForAdminQueryHandler(IAdminQueryService adminQueryService)
    {
        _adminQueryService = adminQueryService;
    }

    public async Task<GetAllReservationsForAdminResult> Handle(GetAllReservationsForAdminQuery request, CancellationToken ct)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        return await _adminQueryService.GetAllReservationsForAdminAsync(
            request.Status,
            page,
            pageSize,
            ct);
    }
}
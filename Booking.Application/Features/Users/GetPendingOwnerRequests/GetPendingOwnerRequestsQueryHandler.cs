using Booking.Application.Abstractions.Queries;
using MediatR;

namespace Booking.Application.Features.Users.GetPendingOwnerRequests;

public sealed class GetPendingOwnerRequestsQueryHandler
    : IRequestHandler<GetPendingOwnerRequestsQuery, GetPendingOwnerRequestsResult>
{
    private readonly IAdminQueryService _adminQueryService;

    public GetPendingOwnerRequestsQueryHandler(IAdminQueryService adminQueryService)
    {
        _adminQueryService = adminQueryService;
    }

    public async Task<GetPendingOwnerRequestsResult> Handle(GetPendingOwnerRequestsQuery request, CancellationToken ct)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        return await _adminQueryService.GetPendingOwnerRequestsAsync(page, pageSize, ct);
    }
}
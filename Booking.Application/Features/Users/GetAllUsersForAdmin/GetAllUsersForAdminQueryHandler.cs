using Booking.Application.Abstractions.Queries;
using MediatR;

namespace Booking.Application.Features.Users.GetAllUsersForAdmin;

public sealed class GetAllUsersForAdminQueryHandler
    : IRequestHandler<GetAllUsersForAdminQuery, GetAllUsersForAdminResult>
{
    private readonly IAdminQueryService _adminQueryService;

    public GetAllUsersForAdminQueryHandler(IAdminQueryService adminQueryService)
    {
        _adminQueryService = adminQueryService;
    }

    public async Task<GetAllUsersForAdminResult> Handle(GetAllUsersForAdminQuery request, CancellationToken ct)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        return await _adminQueryService.GetAllUsersForAdminAsync(page, pageSize, ct);
    }
}
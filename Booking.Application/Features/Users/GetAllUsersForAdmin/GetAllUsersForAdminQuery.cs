
using MediatR;

namespace Booking.Application.Features.Users.GetAllUsersForAdmin;

public sealed record GetAllUsersForAdminQuery(
    int Page,
    int PageSize
) : IRequest<GetAllUsersForAdminResult>;
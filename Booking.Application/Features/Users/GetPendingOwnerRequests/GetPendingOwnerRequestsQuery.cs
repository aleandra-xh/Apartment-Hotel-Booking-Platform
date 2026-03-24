using MediatR;

namespace Booking.Application.Features.Users.GetPendingOwnerRequests;

public sealed record GetPendingOwnerRequestsQuery(int Page = 1, int PageSize = 10)
    : IRequest<GetPendingOwnerRequestsResult>;
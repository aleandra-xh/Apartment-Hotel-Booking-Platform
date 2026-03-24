
namespace Booking.Application.Features.Users.GetPendingOwnerRequests;

public sealed class GetPendingOwnerRequestsResult
{
    public List<PendingOwnerRequestResponse> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
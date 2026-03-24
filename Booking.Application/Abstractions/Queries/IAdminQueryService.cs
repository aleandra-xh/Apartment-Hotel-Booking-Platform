
using Booking.Application.Features.Reservations.GetAllReservationsForAdmin;
using Booking.Application.Features.Users.GetAllUsersForAdmin;
using Booking.Application.Features.Users.GetPendingOwnerRequests;
using Booking.Domain.Reservations;

namespace Booking.Application.Abstractions.Queries;

public interface IAdminQueryService
{
    Task<GetPendingOwnerRequestsResult> GetPendingOwnerRequestsAsync(int page, int pageSize, CancellationToken ct);
    Task<GetAllUsersForAdminResult> GetAllUsersForAdminAsync(int page, int pageSize, CancellationToken ct);
    Task<GetAllReservationsForAdminResult> GetAllReservationsForAdminAsync(
        ReservationStatus? status,
        int page,
        int pageSize,
        CancellationToken ct);
}
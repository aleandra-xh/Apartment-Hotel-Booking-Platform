using Booking.Application.Abstractions.Queries;
using Booking.Application.Features.Reservations.GetAllReservationsForAdmin;
using Booking.Application.Features.Users.GetAllUsersForAdmin;
using Booking.Application.Features.Users.GetPendingOwnerRequests;
using Booking.Domain.OwnerProfiles;
using Booking.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Queries;

public sealed class AdminQueryService : IAdminQueryService
{
    private readonly BookingDbContext _context;

    public AdminQueryService(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<GetPendingOwnerRequestsResult> GetPendingOwnerRequestsAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _context.OwnerProfiles
            .AsNoTracking()
            .Where(op => op.VerificationStatus == VerificationStatus.Pending)
            .Join(
                _context.Users.AsNoTracking().Where(u => !u.IsDeleted),
                op => op.UserId,
                u => u.Id,
                (op, u) => new PendingOwnerRequestResponse
                {
                    UserId = u.Id,
                    FullName = (u.FirstName + " " + u.LastName).Trim(),
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    BusinessName = op.BusinessName,
                    IdentityCardNumber = op.IdentityCardNumber,
                    VerificationStatus = op.VerificationStatus.ToString(),
                    CreatedAt = op.CreatedAt
                });

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new GetPendingOwnerRequestsResult
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<GetAllUsersForAdminResult> GetAllUsersForAdminAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new GetAllUsersForAdminResponse(
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.PhoneNumber,
                u.IsActive,
                u.IsDeleted,
                u.CreatedAt
            ))
            .ToListAsync(ct);
        
        return new GetAllUsersForAdminResult(
            items,
            totalCount,
            page,
    pageSize,
    (int)Math.Ceiling(totalCount / (double)pageSize)
);
    }
    public async Task<GetAllReservationsForAdminResult> GetAllReservationsForAdminAsync(
        ReservationStatus? status,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = _context.Reservations
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(r => r.BookingStatus == status.Value);
        }

        query = query.OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new GetAllReservationsForAdminResponse(
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
            .ToListAsync(ct);

        return new GetAllReservationsForAdminResult(
            items,
            totalCount,
            page,
            pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)
        );
    }

}
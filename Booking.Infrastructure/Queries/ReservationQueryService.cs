
using Booking.Application.Abstractions.Queries;
using Booking.Application.Features.Reservations.GetMyReservations;
using Booking.Application.Features.Reservations.GetOwnerReservations;
using Booking.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Queries;

public sealed class ReservationQueryService : IReservationQueryService
{
    private readonly BookingDbContext _context;

    public ReservationQueryService(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<GetMyReservationsResult> GetMyReservationsAsync(
        Guid guestId,
        ReservationStatus? status,
        bool? isPast,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = _context.Reservations
            .AsNoTracking()
            .Where(r => r.GuestId == guestId);

        query = ApplyReservationFilters(query, status, isPast);

        query = query.OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new GetMyReservationsResponse(
                r.Id,
                r.PropertyId,
                r.StartDate,
                r.EndDate,
                r.GuestCount,
                r.TotalPrice,
                r.BookingStatus,
                r.CreatedAt
            ))
            .ToListAsync(ct);

        return new GetMyReservationsResult(
            items,
            totalCount,
            page,
            pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)
        );
    }

    public async Task<GetOwnerReservationsResult> GetOwnerReservationsAsync(
        Guid ownerId,
        ReservationStatus? status,
        bool? isPast,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var ownerPropertyIds = _context.Properties
            .AsNoTracking()
            .Where(p => p.OwnerId == ownerId)
            .Select(p => p.Id);

        var query = _context.Reservations
            .AsNoTracking()
            .Where(r => ownerPropertyIds.Contains(r.PropertyId));

        query = ApplyReservationFilters(query, status, isPast);

        query = query.OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new GetOwnerReservationsResponse(
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

        return new GetOwnerReservationsResult(
            items,
            totalCount,
            page,
            pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)
        );
    }

    private static IQueryable<Reservation> ApplyReservationFilters(
        IQueryable<Reservation> query,
        ReservationStatus? status,
        bool? isPast)
    {
        if (status.HasValue)
        {
            return query.Where(r => r.BookingStatus == status.Value);
        }

        if (isPast.HasValue)
        {
            if (isPast.Value)
            {
                return query.Where(r => r.BookingStatus == ReservationStatus.Completed);
            }

            return query.Where(r =>
                r.BookingStatus == ReservationStatus.Pending ||
                r.BookingStatus == ReservationStatus.Confirmed);
        }

        return query.Where(r =>
            r.BookingStatus == ReservationStatus.Pending ||
            r.BookingStatus == ReservationStatus.Confirmed ||
            r.BookingStatus == ReservationStatus.Completed);
    }
}
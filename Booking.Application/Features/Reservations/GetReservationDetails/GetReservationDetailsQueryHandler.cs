
using AutoMapper;
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Properties;
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.GetReservationDetails;

public sealed class GetReservationDetailsQueryHandler
    : IRequestHandler<GetReservationDetailsQuery, GetReservationDetailsResponse>
{
    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetReservationDetailsQueryHandler(
        IGenericRepository<Reservation> reservationRepository,
        IGenericRepository<Property> propertyRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<GetReservationDetailsResponse> Handle(GetReservationDetailsQuery request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var reservation = await _reservationRepository.FirstOrDefaultAsync(
            r => r.Id == request.ReservationId,
            ct);

        if (reservation is null)
            throw new NotFoundException("Reservation not found.");

        if (reservation.GuestId == userId)
            return _mapper.Map<GetReservationDetailsResponse>(reservation);

        var property = await _propertyRepository.FirstOrDefaultAsync(
            p => p.Id == reservation.PropertyId,
            ct);

        if (property is null)
            throw new NotFoundException("Property not found.");

        if (property.OwnerId != userId)
            throw new UnauthorizedException("You are not allowed to view this reservation.");

        return _mapper.Map<GetReservationDetailsResponse>(reservation);
    }
}
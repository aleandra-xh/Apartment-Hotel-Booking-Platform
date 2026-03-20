using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace Booking.Application.Features.PropertyBlockedDates.GetPropertyBlockedDates;

public sealed record GetPropertyBlockedDatesQuery(Guid PropertyId)
    : IRequest<List<GetPropertyBlockedDatesResponse>>;
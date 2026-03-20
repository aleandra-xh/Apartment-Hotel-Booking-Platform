using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Features.PropertyBlockedDates.BlockPropertyDates;

public sealed record BlockPropertyDatesRequest(
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason
);
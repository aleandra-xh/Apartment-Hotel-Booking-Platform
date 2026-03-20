
using Booking.Domain.Properties;

namespace Booking.Domain.PropertyBlockedDates;

public class PropertyBlockedDate
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }
}

using Booking.Domain.Properties;

namespace Booking.Domain.PropertySeasonalPrices;

public class PropertySeasonalPrice
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public decimal PricePerNight { get; set; }

    public string? Label { get; set; }

    public DateTime CreatedAt { get; set; }
}
using Booking.Domain.Properties;

namespace Booking.Application.Abstractions.Properties;

public interface IPropertyRepository
{
    Task<Property?> GetPropertyByIdWithDetailsAsync(Guid propertyId, CancellationToken ct = default);
}
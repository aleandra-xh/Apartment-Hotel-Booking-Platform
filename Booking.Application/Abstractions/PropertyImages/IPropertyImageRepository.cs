
namespace Booking.Application.Abstractions.PropertyImages;

public interface IPropertyImageRepository
{
    Task<bool> ExistsByHashAsync(Guid propertyId, string imageHash, CancellationToken ct = default);
    Task<int> CountByPropertyIdAsync(Guid propertyId, CancellationToken ct = default);
}
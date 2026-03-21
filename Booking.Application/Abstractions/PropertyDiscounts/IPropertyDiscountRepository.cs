
using Booking.Domain.PropertyDiscounts;

namespace Booking.Application.Abstractions.PropertyDiscounts;

public interface IPropertyDiscountRepository
{
    Task<bool> HasOverlappingDiscountAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default);

    Task<List<PropertyDiscount>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default);

    Task<PropertyDiscount?> GetByIdAsync(Guid discountId, CancellationToken ct = default);

    Task<decimal?> GetApplicableDiscountPercentageAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default);
}
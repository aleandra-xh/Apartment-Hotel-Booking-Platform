

using Booking.Application.Abstractions.Properties;
using MediatR;

namespace Booking.Application.Features.Properties.SearchProperties;

public sealed class SearchPropertiesQueryHandler
    : IRequestHandler<SearchPropertiesQuery, SearchPropertiesResponse>
{
    private readonly IPropertyRepository _propertyRepository;

    public SearchPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<SearchPropertiesResponse> Handle(SearchPropertiesQuery request, CancellationToken ct)
    {
        var page = request.Request.Page < 1 ? 1 : request.Request.Page;
        var pageSize = request.Request.PageSize < 1 ? 10 : request.Request.PageSize;

        var result = await _propertyRepository.SearchPropertiesAsync(
            request.Request.City,
            request.Request.MaxGuests,
            request.Request.PropertyType,
            request.Request.StartDate,
            request.Request.EndDate,
            request.Request.MinPrice,
            request.Request.MaxPrice,
            request.Request.AmenityIds,
            request.Request.MinRating,
            request.Request.SortBy,
            request.Request.SortDirection,
            page,
            pageSize,
            ct);

        var items = result.Items
            .Select(p => new SearchPropertyItemResponse(
                p.Id,
                p.Name,
                p.Description,
                p.PropertyType.ToString(),
                p.Address.City,
                p.MaxGuests,
                p.IsActive,
                p.IsApproved
            ))
            .ToList();

        return new SearchPropertiesResponse(
            items,
            page,
            pageSize,
            result.TotalCount,
            (int)Math.Ceiling(result.TotalCount / (double)pageSize)
        );
    }
}
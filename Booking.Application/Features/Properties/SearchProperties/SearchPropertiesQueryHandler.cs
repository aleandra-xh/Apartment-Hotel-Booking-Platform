

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
        Console.WriteLine($"Handler City: {request.Request.City}");
        Console.WriteLine($"Handler MaxGuests: {request.Request.MaxGuests}");
        Console.WriteLine($"Handler PropertyType: {request.Request.PropertyType}");
        Console.WriteLine($"Handler StartDate: {request.Request.StartDate}");
        Console.WriteLine($"Handler EndDate: {request.Request.EndDate}");

        var result = await _propertyRepository.SearchPropertiesAsync(
            request.Request.City,
            request.Request.MaxGuests,
            request.Request.PropertyType,
            request.Request.StartDate,
            request.Request.EndDate,
            request.Request.MinPrice,
            request.Request.MaxPrice,
            request.Request.Page,
            request.Request.PageSize,
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
            request.Request.Page,
            request.Request.PageSize,
            result.TotalCount
        );
    }
}

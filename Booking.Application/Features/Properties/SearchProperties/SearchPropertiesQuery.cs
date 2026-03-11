
using MediatR;

namespace Booking.Application.Features.Properties.SearchProperties;

public sealed record SearchPropertiesQuery(SearchPropertiesRequest Request)
    : IRequest<SearchPropertiesResponse>;
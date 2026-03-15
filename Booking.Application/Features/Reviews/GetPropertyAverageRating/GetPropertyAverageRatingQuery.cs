
using MediatR;

namespace Booking.Application.Features.Reviews.GetPropertyAverageRating;

public sealed record GetPropertyAverageRatingQuery(Guid PropertyId) : IRequest<GetPropertyAverageRatingResponse>;
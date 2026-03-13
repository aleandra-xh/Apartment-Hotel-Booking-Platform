
using MediatR;

namespace Booking.Application.Features.Reviews.GetPropertyReviews;

public sealed record GetPropertyReviewsQuery(Guid PropertyId) : IRequest<List<GetPropertyReviewsResponse>>;
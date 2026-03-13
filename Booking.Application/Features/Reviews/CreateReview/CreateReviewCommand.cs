
using MediatR;

namespace Booking.Application.Features.Reviews.CreateReview;

public sealed record CreateReviewCommand(CreateReviewRequest Request) : IRequest<Guid>;
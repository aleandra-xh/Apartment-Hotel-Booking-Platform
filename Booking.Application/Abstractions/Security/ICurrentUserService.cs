namespace Booking.Application.Abstractions.Security;

public interface ICurrentUserService
{
    Guid UserId { get; }
}

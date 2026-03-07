
namespace Booking.Application.Abstractions.CreateProperty;

public interface ICurrentUserService
{
    Guid UserId { get; }
}


namespace Booking.Application.Features.Users.GetAllUsersForAdmin;

public sealed record GetAllUsersForAdminResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    bool IsActive,
    bool IsDeleted,
    DateTime CreatedAt
);
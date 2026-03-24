
namespace Booking.Application.Features.Users.GetMyProfile;

public sealed record GetMyProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? ProfileImageUrl,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastModifiedAt,
    List<string> Roles
);

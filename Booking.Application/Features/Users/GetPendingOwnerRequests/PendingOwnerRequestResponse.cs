
namespace Booking.Application.Features.Users.GetPendingOwnerRequests;

public sealed class PendingOwnerRequestResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? BusinessName { get; set; }
    public string? IdentityCardNumber { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
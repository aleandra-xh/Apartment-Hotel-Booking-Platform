
namespace Booking.Application.Features.BecomeOwner;

public sealed record BecomeOwnerRequest
(
    string IdentityCardNumber,
        string CreditCard,
        string BusinessName
);
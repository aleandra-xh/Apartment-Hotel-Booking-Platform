namespace Booking.Application.Features.Users.BecomeOwner;

public sealed record BecomeOwnerRequest
(
    string IdentityCardNumber,
        string CreditCard,
        string BusinessName
);
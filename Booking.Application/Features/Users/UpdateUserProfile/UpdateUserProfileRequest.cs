using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Features.Users.UpdateUserProfile;

public sealed record UpdateUserProfileRequest(
    string? FirstName,
    string? LastName,
    string? PhoneNumber
);
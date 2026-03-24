
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.OwnerProfiles;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Features.Users.RejectOwnerRequest;

public sealed class RejectOwnerRequestCommandHandler : IRequestHandler<RejectOwnerRequestCommand, Unit>
{
    private readonly IGenericRepository<OwnerProfile> _ownerProfileRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;

    public RejectOwnerRequestCommandHandler(
        IGenericRepository<OwnerProfile> ownerProfileRepository,
        IGenericRepository<User> userRepository,
        INotificationService notificationService,
        IEmailService emailService)
    {
        _ownerProfileRepository = ownerProfileRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(RejectOwnerRequestCommand request, CancellationToken ct)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        var ownerProfile = await _ownerProfileRepository.FirstOrDefaultAsync(
            op => op.UserId == request.UserId,
            ct);

        if (ownerProfile is null)
            throw new NotFoundException("Owner request not found.");

        if (ownerProfile.VerificationStatus != VerificationStatus.Pending)
            throw new ConflictException("Only pending owner requests can be rejected.");

        ownerProfile.VerificationStatus = VerificationStatus.Rejected;
        ownerProfile.LastModifiedAt = DateTime.UtcNow;

        await _ownerProfileRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            request.UserId,
            "Owner request rejected",
            "Your owner request has been rejected. You may update your information and apply again later.",
            NotificationType.OwnerRequestRejected,
            ct);

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            await _emailService.SendAsync(
                new EmailMessage(
                    user.Email,
                    "Owner request rejected",
                    "Your owner request has been rejected. You may review your information and submit a new request later."
                ),
                ct);
        }

        return Unit.Value;
    }
}
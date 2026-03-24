using Booking.Application.Abstractions.Notifications;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Features.Users.SuspendUser;

public sealed class SuspendUserCommandHandler : IRequestHandler<SuspendUserCommand, Unit>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;

    public SuspendUserCommandHandler(
        IGenericRepository<User> userRepository,
        INotificationService notificationService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _notificationService = notificationService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(SuspendUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.IsDeleted)
            throw new ConflictException("Deleted user cannot be suspended.");

        if (!user.IsActive)
            throw new ConflictException("User is already suspended.");

        user.IsActive = false;
        user.LastModifiedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            user.Id,
            "Account suspended",
            "Your account has been suspended by an administrator.",
            NotificationType.AccountSuspended,
            ct);

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            await _emailService.SendAsync(
                new EmailMessage(
                    user.Email,
                    "Account suspended",
                    $"Hello {user.FirstName}, your account has been suspended by an administrator. If you believe this is a mistake, please contact support."
                ),
                ct);
        }

        return Unit.Value;
    }
}


using Booking.Application.Abstractions.Notifications;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.OwnerProfiles;
using Booking.Domain.Roles;
using Booking.Domain.UserRoles;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Features.Admin.RemoveOwnerRole;

public sealed class RemoveOwnerRoleCommandHandler : IRequestHandler<RemoveOwnerRoleCommand, Unit>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IGenericRepository<UserRole> _userRoleRepository;
    private readonly IGenericRepository<OwnerProfile> _ownerProfileRepository;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;

    public RemoveOwnerRoleCommandHandler(
        IGenericRepository<User> userRepository,
        IGenericRepository<Role> roleRepository,
        IGenericRepository<UserRole> userRoleRepository,
        IGenericRepository<OwnerProfile> ownerProfileRepository,
        INotificationService notificationService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _ownerProfileRepository = ownerProfileRepository;
        _notificationService = notificationService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(RemoveOwnerRoleCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.IsDeleted)
            throw new ConflictException("Deleted users cannot be modified.");

        var ownerRole = await _roleRepository.FirstOrDefaultAsync(
            r => r.Name == RoleType.Owner,
            ct);

        if (ownerRole is null)
            throw new NotFoundException("Owner role not found.");

        var ownerUserRole = await _userRoleRepository.FirstOrDefaultAsync(
            ur => ur.UserId == request.UserId && ur.RoleId == ownerRole.Id,
            ct);

        if (ownerUserRole is null)
            throw new ConflictException("User does not have the Owner role.");

        _userRoleRepository.Remove(ownerUserRole);

        var ownerProfile = await _ownerProfileRepository.FirstOrDefaultAsync(
            op => op.UserId == request.UserId,
            ct);

        if (ownerProfile is not null)
        {
            ownerProfile.VerificationStatus = VerificationStatus.Rejected;
            ownerProfile.LastModifiedAt = DateTime.UtcNow;
        }

        await _userRoleRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            request.UserId,
            "Owner access removed",
            "Your owner access has been removed by an administrator.",
            NotificationType.OwnerRequestRejected,
            ct);

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            await _emailService.SendAsync(
                new EmailMessage(
                    user.Email,
                    "Owner access removed",
                    "Your owner access has been removed by an administrator. You can no longer use owner features."
                ),
                ct);
        }

        return Unit.Value;
    }
}
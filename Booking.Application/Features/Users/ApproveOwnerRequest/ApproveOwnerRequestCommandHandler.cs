
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using Booking.Domain.OwnerProfiles;
using Booking.Domain.Roles;
using Booking.Domain.UserRoles;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Features.Users.ApproveOwnerRequest;

public sealed class ApproveOwnerRequestCommandHandler : IRequestHandler<ApproveOwnerRequestCommand, Unit>
{
    private readonly IGenericRepository<OwnerProfile> _ownerProfileRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IGenericRepository<UserRole> _userRoleRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;

    public ApproveOwnerRequestCommandHandler(
        IGenericRepository<OwnerProfile> ownerProfileRepository,
        IGenericRepository<Role> roleRepository,
        IGenericRepository<UserRole> userRoleRepository,
        IGenericRepository<User> userRepository,
        INotificationService notificationService,
        IEmailService emailService)
    {
        _ownerProfileRepository = ownerProfileRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(ApproveOwnerRequestCommand request, CancellationToken ct)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.IsDeleted || !user.IsActive)
            throw new ConflictException("Suspended or deleted users cannot be approved as owners.");

        var ownerProfile = await _ownerProfileRepository.FirstOrDefaultAsync(
            op => op.UserId == request.UserId,
            ct);

        if (ownerProfile is null)
            throw new NotFoundException("Owner request not found.");

        if (ownerProfile.VerificationStatus != VerificationStatus.Pending)
            throw new ConflictException("Only pending owner requests can be approved.");

        var ownerRole = await _roleRepository.FirstOrDefaultAsync(
            r => r.Name == RoleType.Owner,
            ct);

        if (ownerRole is null)
            throw new NotFoundException("Owner role not found.");

        var alreadyOwner = await _userRoleRepository.AnyAsync(
            ur => ur.UserId == request.UserId && ur.RoleId == ownerRole.Id,
            ct);

        if (!alreadyOwner)
        {
            await _userRoleRepository.AddAsync(
                new UserRole
                {
                    UserId = request.UserId,
                    RoleId = ownerRole.Id,
                    AssignedAt = DateTime.UtcNow
                },
                ct);
        }

        ownerProfile.VerificationStatus = VerificationStatus.Approved;
        ownerProfile.LastModifiedAt = DateTime.UtcNow;

        await _ownerProfileRepository.SaveChangesAsync(ct);

        await _notificationService.CreateAsync(
            request.UserId,
            "Owner request approved",
            "Your owner request has been approved. You can now create and manage properties.",
            NotificationType.OwnerRequestApproved,
            ct);

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            await _emailService.SendAsync(
                new EmailMessage(
                    user.Email,
                    "Owner request approved",
                    "Your owner request has been approved. You can now access owner features and create property listings."
                ),
                ct);
        }

        return Unit.Value;
    }
}
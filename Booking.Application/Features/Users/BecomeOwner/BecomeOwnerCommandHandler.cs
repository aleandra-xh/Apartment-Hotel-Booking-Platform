
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.OwnerProfiles;
using Booking.Domain.Roles;
using Booking.Domain.UserRoles;
using MediatR;

namespace Booking.Application.Features.Users.BecomeOwner;

public sealed class BecomeOwnerCommandHandler : IRequestHandler<BecomeOwnerCommand, Unit>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IGenericRepository<UserRole> _userRoleRepository;
    private readonly IGenericRepository<OwnerProfile> _ownerProfileRepository;

    public BecomeOwnerCommandHandler(
        ICurrentUserService currentUserService,
        IGenericRepository<UserRole> userRoleRepository,
        IGenericRepository<OwnerProfile> ownerProfileRepository)
    {
        _currentUserService = currentUserService;
        _userRoleRepository = userRoleRepository;
        _ownerProfileRepository = ownerProfileRepository;
    }

    public async Task<Unit> Handle(BecomeOwnerCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var alreadyOwner = await _userRoleRepository.AnyAsync(
            ur => ur.UserId == userId && ur.Role.Name == RoleType.Owner,
            ct);

        if (alreadyOwner)
            throw new ConflictException("User is already an owner.");

        var existingOwnerProfile = await _ownerProfileRepository.FirstOrDefaultAsync(
            op => op.UserId == userId,
            ct);

        if (existingOwnerProfile is null)
        {
            var ownerProfile = new OwnerProfile
            {
                UserId = userId,
                IdentityCardNumber = request.Request.IdentityCardNumber,
                CreditCard = request.Request.CreditCard,
                BusinessName = request.Request.BusinessName,
                VerificationStatus = VerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _ownerProfileRepository.AddAsync(ownerProfile, ct);
        }
        else
        {
            if (existingOwnerProfile.VerificationStatus == VerificationStatus.Pending)
                throw new ConflictException("You already have a pending owner request.");

            if (existingOwnerProfile.VerificationStatus == VerificationStatus.Approved)
                throw new ConflictException("Your owner request has already been approved.");

            existingOwnerProfile.IdentityCardNumber = request.Request.IdentityCardNumber;
            existingOwnerProfile.CreditCard = request.Request.CreditCard;
            existingOwnerProfile.BusinessName = request.Request.BusinessName;
            existingOwnerProfile.VerificationStatus = VerificationStatus.Pending;
            existingOwnerProfile.LastModifiedAt = DateTime.UtcNow;
        }

        await _ownerProfileRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
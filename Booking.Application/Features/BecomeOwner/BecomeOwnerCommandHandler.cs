using Booking.Application.Abstractions.CreateProperty;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.OwnerProfiles;
using Booking.Domain.Roles;
using Booking.Domain.UserRoles;
using MediatR;

namespace Booking.Application.Features.BecomeOwner;

public sealed class BecomeOwnerCommandHandler : IRequestHandler<BecomeOwnerCommand, Unit>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IGenericRepository<UserRole> _userRoleRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IGenericRepository<OwnerProfile> _ownerProfileRepository;

    public BecomeOwnerCommandHandler(
        ICurrentUserService currentUserService,
        IGenericRepository<UserRole> userRoleRepository,
        IGenericRepository<Role> roleRepository,
        IGenericRepository<OwnerProfile> ownerProfileRepository)
    {
        _currentUserService = currentUserService;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _ownerProfileRepository = ownerProfileRepository;
    }

    public async Task<Unit> Handle(BecomeOwnerCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var ownerRole = await _roleRepository.FirstOrDefaultAsync(
            r => r.Name == RoleType.Owner,
            ct);

        if (ownerRole is null)
            throw new NotFoundException("Owner role not found.");

        var alreadyOwner = await _userRoleRepository.AnyAsync(
            ur => ur.UserId == userId && ur.RoleId == ownerRole.Id,
            ct);

        if (alreadyOwner)
            throw new ConflictException("User is already an owner.");

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

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = ownerRole.Id,
            AssignedAt = DateTime.UtcNow
        };

        await _userRoleRepository.AddAsync(userRole, ct);
        await _userRoleRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
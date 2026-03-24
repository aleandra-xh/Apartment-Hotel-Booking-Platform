
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Behaviors;

public sealed class ActiveUserCheckBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IGenericRepository<User> _userRepository;

    public ActiveUserCheckBehavior(
        ICurrentUserService currentUserService,
        IGenericRepository<User> userRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (_currentUserService.UserId == Guid.Empty)
            return await next();

        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == _currentUserService.UserId,
            ct);

        if (user is null)
            throw new UnauthorizedException("User not found.");

        if (!user.IsActive)
            throw new UnauthorizedException("Your account has been suspended.");

        return await next();
    }
}

using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Abstractions.UserRegister;

using MediatR;

namespace Booking.Application.Features.Users.UploadUserProfileImage;

public sealed class UploadUserProfileImageCommandHandler : IRequestHandler<UploadUserProfileImageCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UploadUserProfileImageCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<string> Handle(UploadUserProfileImageCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile-images");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            var oldFileName = Path.GetFileName(user.ProfileImageUrl);
            var oldFilePath = Path.Combine(uploadsFolder, oldFileName);

            if (File.Exists(oldFilePath))
                File.Delete(oldFilePath);
        }

        var extension = Path.GetExtension(request.Request.FileName);
        var newFileName = $"{Guid.NewGuid()}{extension}";
        var newFilePath = Path.Combine(uploadsFolder, newFileName);

        await File.WriteAllBytesAsync(newFilePath, request.Request.ImageData, ct);

        user.ProfileImageUrl = $"/profile-images/{newFileName}";

        await _userRepository.SaveChangesAsync(ct);

        return user.ProfileImageUrl;
    }
}
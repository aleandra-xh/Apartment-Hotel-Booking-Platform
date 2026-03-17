using Booking.Domain.OwnerProfiles;
using Booking.Domain.Reviews;
using Booking.Domain.UserRoles;
using Booking.Domain.Reservations;
using System.ComponentModel.DataAnnotations;
using Booking.Domain.Properties;

namespace Booking.Domain.Users;

public class User
{
    [Key]
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string Password { get; private set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }

    // Navigation Properties
    public List<UserRole> UserRoles { get; set; } = new();
    public OwnerProfile? OwnerProfile { get; set; } 
    public List<Reservation> Reservations { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<Property> Properties { get; set; } = new();

    public User(Guid id, string firstName, string lastName, string email, string passwordHash, string phoneNumber)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = passwordHash;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        
    }
    private User() { }

    public static User CreateUser(CreateUserDto dto, string passwordHash)
        => new(
            Guid.NewGuid(), 
            dto.FirstName, 
            dto.LastName, 
            dto.Email, 
            passwordHash, 
            dto.PhoneNumber);

    public void UpdatePassword(string passwordHash)
    {
        Password = passwordHash;
        LastModifiedAt = DateTime.UtcNow;
    }

}
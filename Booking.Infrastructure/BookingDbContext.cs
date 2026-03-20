using Booking.Domain.Addresses;
using Booking.Domain.OwnerProfiles;
using Booking.Domain.Properties;
using Booking.Domain.PropertyAmenities;
using Booking.Domain.PropertyBlockedDates;
using Booking.Domain.PropertyImages;
using Booking.Domain.Reservations;
using Booking.Domain.Reviews;
using Booking.Domain.Roles;
using Booking.Domain.UserRoles;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<OwnerProfile> OwnerProfiles { get; set; } = null!;
        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<PropertyImage> PropertyImages { get; set; } = null!;

        public DbSet<PropertyAmenity> PropertyAmenities { get; set; } = null!;
        public DbSet<PropertyBlockedDate> PropertyBlockedDates => Set<PropertyBlockedDate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = RoleType.Guest,
                    Description = "Guest role",
                    IsDefault = true
                },
                new Role
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = RoleType.Owner,
                    Description = "Owner role",
                    IsDefault = false
                },
                new Role
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = RoleType.Admin,
                    Description = "Admin role",
                    IsDefault = false
                }
        );

            // USER ROLE 
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // OWNER PROFILE 
            modelBuilder.Entity<OwnerProfile>()
                .HasKey(op => op.UserId);

            modelBuilder.Entity<OwnerProfile>()
                .HasOne(op => op.User)
                .WithOne(u => u.OwnerProfile)
                .HasForeignKey<OwnerProfile>(op => op.UserId);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
 
            // PROPERTY → OWNER 
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            // NO DUPLICATE
            modelBuilder.Entity<Property>()
                .HasIndex(p => new { p.OwnerId, p.Name })
                .IsUnique();

            // PROPERTY → ADDRESS 
            modelBuilder.Entity<Property>()
           .HasOne(p => p.Address)
           .WithMany(a => a.Properties)
           .HasForeignKey(p => p.AddressId)
           .OnDelete(DeleteBehavior.Restrict);

            // PROPERTY TYPE
            modelBuilder.Entity<Property>()
                .Property(p => p.PropertyType)
                .HasConversion<int>();

            // PROPERTY AMENITY
            modelBuilder.Entity<PropertyAmenity>(entity =>
            {
                entity.ToTable("PropertyAmenities");

                entity.HasKey(x => new { x.PropertyId, x.Amenity });

                entity.Property(x => x.Amenity)
                      .HasConversion<int>();

                entity.HasOne(x => x.Property)
                      .WithMany(p => p.Amenities)
                      .HasForeignKey(x => x.PropertyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            //PROPERTY → IMAGES
            modelBuilder.Entity<PropertyImage>()
            .HasOne(pi => pi.Property)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyImage>()
                .Property(pi => pi.ImageData)
                .HasColumnType("varbinary(max)");

            modelBuilder.Entity<PropertyImage>()
                .Property(pi => pi.FileName)
                .HasMaxLength(255);

            modelBuilder.Entity<PropertyImage>()
                .Property(pi => pi.ContentType)
                .HasMaxLength(100);

            //IMAGE HASH 
            modelBuilder.Entity<PropertyImage>()
                .Property(pi => pi.ImageHash)
                .HasMaxLength(64);


            // RESERVATION → GUEST 
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Guest)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.NoAction);

            // RESERVATION → PROPERTY
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Property)
                .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.BookingStatus)
                .HasConversion<int>();

            // DECIMAL PRECISION
            modelBuilder.Entity<Reservation>()
                .Property(r => r.CleaningFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.AmenitiesUpCharge)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.PriceForPeriod)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.AdditionalGuestFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.ServiceFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.TaxAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.RefundAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.PenaltyAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Property>()
                .Property(p => p.PricePerNight)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Property>()
                .Property(p => p.CleaningFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Property>()
                .Property(p => p.ServiceFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Property>()
                .Property(p => p.TaxPercentage)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Property>()
                .Property(p => p.AdditionalGuestFeePerNight)
                .HasPrecision(18, 2);

            // REVIEW → BOOKING 
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reservation)
                .WithOne(b => b.Review)
                .HasForeignKey<Review>(r => r.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            // REVIEW → GUEST
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Guest)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.NoAction);

            //PROPERTYBLOCKEDDATES → PROPERTY
            modelBuilder.Entity<PropertyBlockedDate>()
                .HasOne(pbd => pbd.Property)
                .WithMany(p => p.BlockedDates)
                .HasForeignKey(pbd => pbd.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyBlockedDate>()
                .HasIndex(pbd => pbd.PropertyId);
        }

    }
}



using Booking.Domain.Properties;

namespace Booking.Domain.PropertyImages;

public class PropertyImage
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public byte[] ImageData { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string ImageHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}



using AutoMapper;
using Booking.Application.Features.PropertyImages.GetPropertyImages;
using Booking.Application.Features.Reservations.GetMyReservations;
using Booking.Application.Features.Reservations.GetOwnerReservations;
using Booking.Application.Features.Reviews.GetPropertyReviews;
using Booking.Domain.PropertyImages;
using Booking.Domain.Reservations;
using Booking.Domain.Reviews;


namespace Booking.Application.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Reservation, GetMyReservationsResponse>();

        CreateMap<Reservation, GetOwnerReservationsResponse>();

        CreateMap<Review, GetPropertyReviewsResponse>();

        CreateMap<PropertyImage, GetPropertyImagesResponse>()
            .ForCtorParam(
                "Base64Image",
                opt => opt.MapFrom(src => Convert.ToBase64String(src.ImageData))
            );
    }
}

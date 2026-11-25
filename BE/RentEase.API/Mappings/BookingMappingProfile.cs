using AutoMapper;
using RentEase.API.Entities;
using RentEase.API.DTO.Bookings;

namespace RentEase.API.Mappings;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, BookingResponse>();

    }
}
using AutoMapper;
using ConferenceBooking.Bll.Common.Bookings.Models;
using ConferenceBooking.Bll.Common.Rooms.Models;
using ConferenceBooking.Dal.SqlRepositories.Bookings.Entities;
using ConferenceBooking.Dal.SqlRepositories.Rooms.Entities;

namespace ConferenceBooking.Dal.SqlRepositories.Mapping;

public class DalMappingProfile : Profile
{
    public DalMappingProfile()
    {
        CreateMap<RoomEntity, Room>()
            .ForMember(dest => dest.AvailableServices, opt => opt.Ignore());
        CreateMap<Room, RoomEntity>();

        CreateMap<ServiceEntity, Service>();
        CreateMap<Service, ServiceEntity>();

        CreateMap<BookingEntity, Booking>()
            .ForMember(dest => dest.SelectedServiceIds, opt => opt.Ignore());
        CreateMap<Booking, BookingEntity>();
    }
}

using AutoMapper;
using ConferenceBookingApi.DTOs.Bookings;
using ConferenceBookingApi.DTOs.Rooms;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Room, RoomResponseDto>();
        CreateMap<Service, ServiceResponseDto>();

        CreateMap<Booking, BookingResponseDto>()
            .ForMember(dest => dest.DurationHours, opt => opt.MapFrom(src => (src.EndTime - src.StartTime).TotalHours));

        CreateMap<(Booking Booking, Room? Room), BookingResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Booking.Id))
            .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.Booking.RoomId))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Booking.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Booking.EndTime))
            .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => src.Booking.TotalCost))
            .ForMember(dest => dest.DurationHours, opt => opt.MapFrom(src => (src.Booking.EndTime - src.Booking.StartTime).TotalHours))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room != null ? src.Room.Name : "Невідомо"))
            .ForMember(dest => dest.SelectedServices, opt => opt.MapFrom(src =>
                src.Room != null
                    ? src.Room.AvailableServices
                        .Where(s => src.Booking.SelectedServiceIds.Contains(s.Id))
                        .Select(s => s.Name)
                        .ToList()
                    : new List<string>()));
    }
}

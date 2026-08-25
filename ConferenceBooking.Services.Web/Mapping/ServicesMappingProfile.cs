using AutoMapper;
using ConferenceBooking.Bll.Common.Bookings.Models;
using ConferenceBooking.Bll.Common.Reports.Models;
using ConferenceBooking.Bll.Common.Rooms.Models;
using ConferenceBooking.Services.Web.DTOs.Bookings;
using ConferenceBooking.Services.Web.DTOs.Reports;
using ConferenceBooking.Services.Web.DTOs.Rooms;

namespace ConferenceBooking.Services.Web.Mapping;

public class ServicesMappingProfile : Profile
{
    public ServicesMappingProfile()
    {
        // Rooms
        CreateMap<CreateRoomDto, Room>()
            .ForMember(dest => dest.AvailableServices, opt => opt.MapFrom(src =>
                src.AvailableServices.Select(s => new Service { Name = s.Name, Price = s.Price })));

        CreateMap<UpdateRoomDto, Room>()
            .ForMember(dest => dest.AvailableServices, opt => opt.MapFrom(src =>
                src.AvailableServices.Select(s => new Service { Id = s.Id ?? Guid.Empty, Name = s.Name, Price = s.Price })));

        CreateMap<Room, RoomResponseDto>();
        CreateMap<Service, ServiceResponseDto>();

        // Bookings
        CreateMap<PriceBreakdownItem, PriceBreakdownItemDto>();
        CreateMap<PricingResult, PricingResultDto>();

        CreateMap<Booking, BookingResponseDto>();

        CreateMap<BookingDetails, BookingResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Booking.Id))
            .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.Booking.RoomId))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room.Name))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Booking.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Booking.EndTime))
            .ForMember(dest => dest.SelectedServices, opt => opt.MapFrom(src =>
                src.Room.AvailableServices.Where(s => src.Booking.SelectedServiceIds.Contains(s.Id))))
            .ForMember(dest => dest.RoomCost, opt => opt.MapFrom(src => src.Pricing != null ? src.Pricing.RoomCost : 0))
            .ForMember(dest => dest.ServicesCost, opt => opt.MapFrom(src => src.Pricing != null ? src.Pricing.ServicesCost : 0))
            .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => src.Booking.TotalCost))
            .ForMember(dest => dest.PriceBreakdown, opt => opt.MapFrom(src => src.Pricing != null ? src.Pricing.Breakdown : new List<PriceBreakdownItem>()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Booking.CreatedAt));

        // Reports
        CreateMap<RevenueReport, RevenueReportDto>();
        CreateMap<DailyRevenue, DailyRevenueDto>();
        CreateMap<RoomPopularity, RoomPopularityDto>();
        CreateMap<RoomLoad, RoomLoadDto>();
    }
}

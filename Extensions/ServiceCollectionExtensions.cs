using ConferenceBookingApi.Data.Repositories;
using ConferenceBookingApi.Data.Repositories.Interfaces;
using ConferenceBookingApi.Mappings;
using ConferenceBookingApi.Services;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();
        services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();

        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IReportService, ReportService>();

        services.AddSingleton<IPricingService, PricingService>();

        return services;
    }
}

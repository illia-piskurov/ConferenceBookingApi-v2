using ConferenceBookingApi.Repositories;
using ConferenceBookingApi.Repositories.Interfaces;
using ConferenceBookingApi.Services;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();
        services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();

        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IReportService, ReportService>();

        services.AddSingleton<PricingService>();

        return services;
    }
}

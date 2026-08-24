using ConferenceBookingApi.Data;
using ConferenceBookingApi.Data.Repositories;
using ConferenceBookingApi.Data.Repositories.Interfaces;
using ConferenceBookingApi.Mappings;
using ConferenceBookingApi.Services;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        services.AddScoped<IRoomRepository, SqlRoomRepository>();
        services.AddScoped<IBookingRepository, SqlBookingRepository>();

        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IReportService, ReportService>();

        services.AddSingleton<IPricingService, PricingService>();

        return services;
    }
}

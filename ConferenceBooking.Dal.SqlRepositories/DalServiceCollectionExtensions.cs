using ConferenceBooking.Bll.Common.Bookings;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Dal.SqlRepositories.Bookings;
using ConferenceBooking.Dal.SqlRepositories.Connection;
using ConferenceBooking.Dal.SqlRepositories.Mapping;
using ConferenceBooking.Dal.SqlRepositories.Rooms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceBooking.Dal.SqlRepositories;

public static class DalServiceCollectionExtensions
{
    public static IServiceCollection AddSqlRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddAutoMapper(cfg => cfg.AddProfile<DalMappingProfile>());

        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        services.AddScoped<IRoomRepository, SqlRoomRepository>();
        services.AddScoped<IBookingRepository, SqlBookingRepository>();

        return services;
    }
}

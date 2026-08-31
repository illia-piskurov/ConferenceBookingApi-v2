using ConferenceBooking.Bll.Common.Bookings;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Dal.SqlRepositories.Bookings;
using ConferenceBooking.Dal.SqlRepositories.Configuration;
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

        var schema = configuration[$"{SqlDatabaseOptions.SectionName}:Schema"] ?? "dbo";
        var databaseOptions = new SqlDatabaseOptions { Schema = schema };

        services.AddSingleton(databaseOptions);
        services.AddAutoMapper(cfg => cfg.AddProfile<DalMappingProfile>());

        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString, databaseOptions));

        services.AddScoped<IRoomRepository, SqlRoomRepository>();
        services.AddScoped<IBookingRepository, SqlBookingRepository>();

        return services;
    }
}

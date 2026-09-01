using ConferenceBooking.Bll.Bookings;
using ConferenceBooking.Bll.Common.Bookings;
using ConferenceBooking.Bll.Common.Reports;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Reports;
using ConferenceBooking.Bll.Rooms;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceBooking.Bll;

public static class BllServiceCollectionExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        services.AddScoped<IRoomManager, RoomManager>();
        services.AddScoped<IBookingManager, BookingManager>();
        services.AddScoped<IReportManager, ReportManager>();
        services.AddSingleton<IPricingManager, PricingManager>();

        return services;
    }
}

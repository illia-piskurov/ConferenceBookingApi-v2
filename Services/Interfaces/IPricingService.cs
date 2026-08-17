using ConferenceBookingApi.DTOs.Bookings;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Services.Interfaces;

public interface IPricingService
{
    PricingResultDto Calculate(Room room, DateTime startTime, DateTime endTime, List<Service> selectedServices);
}

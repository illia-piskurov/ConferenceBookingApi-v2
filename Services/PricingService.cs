using ConferenceBookingApi.DTOs.Bookings;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Services;

public class PricingService
{
    private static readonly List<(string Name, TimeOnly Start, TimeOnly End, decimal Multiplier)> PriceZones =
    [
        ("Ранкова знижка",   new TimeOnly(6, 0),  new TimeOnly(9, 0),  0.90m),
        ("Стандарт (ранок)", new TimeOnly(9, 0),  new TimeOnly(12, 0), 1.00m),
        ("Пік",              new TimeOnly(12, 0), new TimeOnly(14, 0), 1.15m),
        ("Стандарт (день)",  new TimeOnly(14, 0), new TimeOnly(18, 0), 1.00m),
        ("Вечірня знижка",   new TimeOnly(18, 0), new TimeOnly(23, 0), 0.80m),
    ];

    public (decimal RoomCost, decimal ServicesCost, decimal TotalCost, List<PriceBreakdownItemDto> Breakdown)
        Calculate(Room room, DateTime startTime, DateTime endTime, List<Service> selectedServices)
    {
        var breakdown = new List<PriceBreakdownItemDto>();
        decimal roomCost = 0;

        var currentDay = startTime.Date;
        var lastDay = endTime.Date;

        while (currentDay <= lastDay)
        {
            var dayStart = currentDay == startTime.Date ? startTime : currentDay;
            var dayEnd = currentDay == lastDay ? endTime : currentDay.AddDays(1);

            foreach (var zone in PriceZones)
            {
                var zoneStart = currentDay.Add(zone.Start.ToTimeSpan());
                var zoneEnd = currentDay.Add(zone.End.ToTimeSpan());

                var intersectStart = zoneStart < dayStart ? dayStart : zoneStart;
                var intersectEnd = zoneEnd > dayEnd ? dayEnd : zoneEnd;

                if (intersectStart >= intersectEnd) continue;

                var hours = (decimal)(intersectEnd - intersectStart).TotalHours;
                var subtotal = hours * room.BaseHourlyRate * zone.Multiplier;
                roomCost += subtotal;

                breakdown.Add(new PriceBreakdownItemDto
                {
                    ZoneName = zone.Name,
                    From = intersectStart,
                    To = intersectEnd,
                    Hours = (double)hours,
                    Rate = room.BaseHourlyRate,
                    Multiplier = zone.Multiplier,
                    Subtotal = subtotal
                });
            }

            currentDay = currentDay.AddDays(1);
        }

        decimal servicesCost = selectedServices.Sum(s => s.Price);
        decimal totalCost = roomCost + servicesCost;

        return (roomCost, servicesCost, totalCost, breakdown);
    }
}

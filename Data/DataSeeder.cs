using ConferenceBookingApi.Models;
using ConferenceBookingApi.Repositories.Interfaces;

namespace ConferenceBookingApi.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roomRepository = serviceProvider.GetRequiredService<IRoomRepository>();

        var projector = new Service { Id = Guid.NewGuid(), Name = "Проєктор", Price = 500 };
        var wifi      = new Service { Id = Guid.NewGuid(), Name = "Wi-Fi",    Price = 300 };
        var sound     = new Service { Id = Guid.NewGuid(), Name = "Звук",     Price = 700 };

        var rooms = new List<Room>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Зал А",
                Capacity = 50,
                BaseHourlyRate = 2000,
                AvailableServices = [projector, wifi, sound]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Зал B",
                Capacity = 100,
                BaseHourlyRate = 3500,
                AvailableServices = [projector, wifi, sound]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Зал C",
                Capacity = 30,
                BaseHourlyRate = 1500,
                AvailableServices = [projector, wifi]
            }
        };

        foreach (var room in rooms)
        {
            await roomRepository.AddAsync(room);
        }
    }
}

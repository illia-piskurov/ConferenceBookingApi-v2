namespace ConferenceBookingApi.DTOs.Rooms;

public class ServiceResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class RoomResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourlyRate { get; set; }
    public List<ServiceResponseDto> AvailableServices { get; set; } = new();
}

using System.ComponentModel.DataAnnotations;

namespace ConferenceBookingApi.DTOs.Rooms;

public class CreateServiceDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
}

public class CreateRoomDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 10000)]
    public int Capacity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal BaseHourlyRate { get; set; }

    public List<CreateServiceDto> Services { get; set; } = new();
}

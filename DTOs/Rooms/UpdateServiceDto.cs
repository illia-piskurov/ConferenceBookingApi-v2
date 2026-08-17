using System.ComponentModel.DataAnnotations;

namespace ConferenceBookingApi.DTOs.Rooms;

public class UpdateServiceDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
}

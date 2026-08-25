using System.ComponentModel.DataAnnotations;

namespace ConferenceBooking.Services.Web.DTOs.Rooms;

public class UpdateRoomDto
{
    [Required(ErrorMessage = "Назва залу є обов'язковою.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва залу має містити від 2 до 100 символів.")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "Місткість залу має бути від 1 до 1000 осіб.")]
    public int Capacity { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "Погодинна ставка має бути більшою за 0.")]
    public decimal BaseHourlyRate { get; set; }

    public List<UpdateServiceDto> AvailableServices { get; set; } = [];
}

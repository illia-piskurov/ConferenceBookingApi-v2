using System.ComponentModel.DataAnnotations;

namespace ConferenceBooking.Services.Web.DTOs.Rooms;

public class UpdateServiceDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Назва послуги є обов'язковою.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва послуги має містити від 2 до 100 символів.")]
    public string Name { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "Вартість послуги не може бути меншою за 0.")]
    public decimal Price { get; set; }
}

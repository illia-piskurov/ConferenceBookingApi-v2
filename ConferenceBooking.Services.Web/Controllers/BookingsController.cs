using AutoMapper;
using ConferenceBooking.Bll.Common.Bookings;
using ConferenceBooking.Services.Web.DTOs.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Services.Web.Controllers;

/// <summary>
/// Контролер для створення та перегляду бронювань конференц-залів.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private readonly IBookingManager _bookingManager;
    private readonly IMapper _mapper;

    /// <summary>
    /// Ініціалізує новий екземпляр <see cref="BookingsController"/>.
    /// </summary>
    /// <param name="bookingManager">Сервіс бізнес-логіки управління бронюваннями.</param>
    /// <param name="mapper">Екземпляр мапера об'єктів.</param>
    public BookingsController(IBookingManager bookingManager, IMapper mapper)
    {
        _bookingManager = bookingManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Створити нове бронювання конференц-залу з автоматичним розрахунком вартості.
    /// </summary>
    /// <param name="dto">Модель даних для оформлення бронювання.</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Детальна інформація про створене бронювання з фінансовою деталізацією.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        var details = await _bookingManager.CreateBookingAsync(
            dto.RoomId, dto.StartTime, dto.EndTime, dto.SelectedServiceIds, cancellationToken);

        var response = _mapper.Map<BookingResponseDto>(details);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Отримати детальну інформацію про бронювання за його ідентифікатором.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор бронювання (GUID).</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Деталі бронювання або 404, якщо запис не знайдено.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var details = await _bookingManager.GetBookingByIdAsync(id, cancellationToken);
        return Ok(_mapper.Map<BookingResponseDto>(details));
    }

    /// <summary>
    /// Отримати список усіх бронювань для конкретного конференц-залу.
    /// </summary>
    /// <param name="roomId">Унікальний ідентифікатор залу (GUID).</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Колекція бронювань вказаного залу.</returns>
    [HttpGet("room/{roomId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByRoom(Guid roomId, CancellationToken cancellationToken = default)
    {
        var detailsList = await _bookingManager.GetBookingsByRoomAsync(roomId, cancellationToken);
        return Ok(_mapper.Map<IEnumerable<BookingResponseDto>>(detailsList));
    }
}

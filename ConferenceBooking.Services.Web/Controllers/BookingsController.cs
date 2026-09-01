using AutoMapper;
using ConferenceBooking.Bll.Common.Bookings;
using ConferenceBooking.Services.Web.DTOs.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Services.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private readonly IBookingManager _bookingManager;
    private readonly IMapper _mapper;

    public BookingsController(IBookingManager bookingManager, IMapper mapper)
    {
        _bookingManager = bookingManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Створити нове бронювання
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
    {
        var details = await _bookingManager.CreateBookingAsync(
            dto.RoomId, dto.StartTime, dto.EndTime, dto.SelectedServiceIds);

        var response = _mapper.Map<BookingResponseDto>(details);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Отримати деталі бронювання за ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var details = await _bookingManager.GetBookingByIdAsync(id);
        return Ok(_mapper.Map<BookingResponseDto>(details));
    }

    /// <summary>
    /// Отримати всі бронювання для конкретного залу
    /// </summary>
    [HttpGet("room/{roomId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByRoom(Guid roomId)
    {
        var detailsList = await _bookingManager.GetBookingsByRoomAsync(roomId);
        return Ok(_mapper.Map<IEnumerable<BookingResponseDto>>(detailsList));
    }
}

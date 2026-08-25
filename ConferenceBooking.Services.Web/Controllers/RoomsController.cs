using AutoMapper;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Common.Rooms.Models;
using ConferenceBooking.Services.Web.DTOs.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Services.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RoomsController : ControllerBase
{
    private readonly IRoomManager _roomManager;
    private readonly IMapper _mapper;

    public RoomsController(IRoomManager roomManager, IMapper mapper)
    {
        _roomManager = roomManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Отримати список усіх залів
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoomResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _roomManager.GetAllRoomsAsync();
        return Ok(_mapper.Map<IEnumerable<RoomResponseDto>>(rooms));
    }

    /// <summary>
    /// Отримати зал за ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var room = await _roomManager.GetRoomByIdAsync(id);
        return Ok(_mapper.Map<RoomResponseDto>(room));
    }

    /// <summary>
    /// Створити новий зал
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
    {
        var roomModel = _mapper.Map<Room>(dto);
        var created = await _roomManager.CreateRoomAsync(roomModel);
        var response = _mapper.Map<RoomResponseDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Оновити дані залу
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomDto dto)
    {
        var roomModel = _mapper.Map<Room>(dto);
        var updated = await _roomManager.UpdateRoomAsync(id, roomModel);
        return Ok(_mapper.Map<RoomResponseDto>(updated));
    }

    /// <summary>
    /// Видалити зал (Soft Delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roomManager.DeleteRoomAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Пошук доступних залів за часовим інтервалом та місткістю
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<RoomResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAvailable(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromQuery] int capacity = 1)
    {
        var available = await _roomManager.SearchAvailableRoomsAsync(start, end, capacity);
        return Ok(_mapper.Map<IEnumerable<RoomResponseDto>>(available));
    }
}

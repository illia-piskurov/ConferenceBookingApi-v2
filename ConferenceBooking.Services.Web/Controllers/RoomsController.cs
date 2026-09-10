using AutoMapper;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Common.Rooms.Models;
using ConferenceBooking.Services.Web.DTOs.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Services.Web.Controllers;

/// <summary>
/// Контролер для управління конференц-залами та їх додатковими послугами.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RoomsController : ControllerBase
{
    private readonly IRoomManager _roomManager;
    private readonly IMapper _mapper;

    /// <summary>
    /// Ініціалізує новий екземпляр <see cref="RoomsController"/>.
    /// </summary>
    /// <param name="roomManager">Сервіс бізнес-логіки конференц-залів.</param>
    /// <param name="mapper">Екземпляр мапера об'єктів.</param>
    public RoomsController(IRoomManager roomManager, IMapper mapper)
    {
        _roomManager = roomManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Отримати список усіх активних конференц-залів.
    /// </summary>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Колекція залів із доступними послугами.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoomResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var rooms = await _roomManager.GetAllRoomsAsync(cancellationToken);
        return Ok(_mapper.Map<IEnumerable<RoomResponseDto>>(rooms));
    }

    /// <summary>
    /// Отримати детальну інформацію про конференц-зал за його унікальним ідентифікатором.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор залу (GUID).</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Дані залу або 404, якщо зал не знайдено.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await _roomManager.GetRoomByIdAsync(id, cancellationToken: cancellationToken);
        return Ok(_mapper.Map<RoomResponseDto>(room));
    }

    /// <summary>
    /// Створити новий конференц-зал.
    /// </summary>
    /// <param name="dto">Модель даних для створення залу.</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Створений зал із присвоєним унікальним ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var request = _mapper.Map<CreateRoomRequest>(dto);
        var created = await _roomManager.CreateRoomAsync(request, cancellationToken);
        var response = _mapper.Map<RoomResponseDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Оновити дані існуючого конференц-залу.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор залу для оновлення.</param>
    /// <param name="dto">Нові дані залу.</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Оновлена інформація про зал.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var request = _mapper.Map<UpdateRoomRequest>(dto);
        var updated = await _roomManager.UpdateRoomAsync(id, request, cancellationToken);
        return Ok(_mapper.Map<RoomResponseDto>(updated));
    }

    /// <summary>
    /// Видалити конференц-зал (м'яке видалення).
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор залу.</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Статус 204 NoContent у разі успішного видалення.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await _roomManager.DeleteRoomAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Пошук доступних конференц-залів за вказаним інтервалом часу та мінімальною місткістю.
    /// </summary>
    /// <param name="start">Дата та час початку запланованого заходу.</param>
    /// <param name="end">Дата та час закінчення запланованого заходу.</param>
    /// <param name="capacity">Необхідна мінімальна місткість (кількість осіб).</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Список вільних залів, що задовольняють критерії пошуку.</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<RoomResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAvailable(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromQuery] int capacity = 1,
        CancellationToken cancellationToken = default)
    {
        var available = await _roomManager.SearchAvailableRoomsAsync(start, end, capacity, cancellationToken);
        return Ok(_mapper.Map<IEnumerable<RoomResponseDto>>(available));
    }
}

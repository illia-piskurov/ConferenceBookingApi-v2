using AutoMapper;
using ConferenceBooking.Bll.Common.Reports;
using ConferenceBooking.Services.Web.DTOs.Reports;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Services.Web.Controllers;

/// <summary>
/// Контролер для формування аналітичних та фінансових звітів.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IReportManager _reportManager;
    private readonly IMapper _mapper;

    /// <summary>
    /// Ініціалізує новий екземпляр <see cref="ReportsController"/>.
    /// </summary>
    /// <param name="reportManager">Сервіс формування бізнес-звітів.</param>
    /// <param name="mapper">Екземпляр мапера об'єктів.</param>
    public ReportsController(IReportManager reportManager, IMapper mapper)
    {
        _reportManager = reportManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Отримати зведений фінансовий звіт про доходи за вказаний період із поденною розбивкою.
    /// </summary>
    /// <param name="from">Початкова дата інтервалу аналізу.</param>
    /// <param name="to">Кінцева дата інтервалу аналізу.</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Звіт із загальною сумою виручки та щоденною статистикою.</returns>
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(RevenueReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRevenueReport([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken cancellationToken = default)
    {
        var report = await _reportManager.GetRevenueReportAsync(from, to, cancellationToken);
        return Ok(_mapper.Map<RevenueReportDto>(report));
    }

    /// <summary>
    /// Отримати рейтинг конференц-залів за популярністю (кількість бронювань, загальна виручка, середня тривалість).
    /// </summary>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Список показників популярності кожного залу.</returns>
    [HttpGet("popularity")]
    [ProducesResponseType(typeof(IEnumerable<RoomPopularityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomPopularity(CancellationToken cancellationToken = default)
    {
        var popularity = await _reportManager.GetRoomPopularityAsync(cancellationToken);
        return Ok(_mapper.Map<IEnumerable<RoomPopularityDto>>(popularity));
    }

    /// <summary>
    /// Отримати звіт про завантаженість конференц-залів (відсоток використання) за вказаний період.
    /// </summary>
    /// <param name="from">Початкова дата інтервалу аналізу.</param>
    /// <param name="to">Кінцева дата інтервалу аналізу.</param>
    /// <param name="cancellationToken">Токен скасування запиту.</param>
    /// <returns>Список залів із кількістю заброньованих годин та відсотком завантаженості.</returns>
    [HttpGet("load")]
    [ProducesResponseType(typeof(IEnumerable<RoomLoadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoomLoad([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken cancellationToken = default)
    {
        var load = await _reportManager.GetRoomLoadAsync(from, to, cancellationToken);
        return Ok(_mapper.Map<IEnumerable<RoomLoadDto>>(load));
    }
}

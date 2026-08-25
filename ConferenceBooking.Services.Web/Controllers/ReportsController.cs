using AutoMapper;
using ConferenceBooking.Bll.Common.Reports;
using ConferenceBooking.Services.Web.DTOs.Reports;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Services.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IReportManager _reportManager;
    private readonly IMapper _mapper;

    public ReportsController(IReportManager reportManager, IMapper mapper)
    {
        _reportManager = reportManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Звіт про доходи за період
    /// </summary>
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(RevenueReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRevenueReport([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var report = await _reportManager.GetRevenueReportAsync(from, to);
        return Ok(_mapper.Map<RevenueReportDto>(report));
    }

    /// <summary>
    /// Рейтинг залів за популярністю
    /// </summary>
    [HttpGet("popularity")]
    [ProducesResponseType(typeof(IEnumerable<RoomPopularityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomPopularity()
    {
        var popularity = await _reportManager.GetRoomPopularityAsync();
        return Ok(_mapper.Map<IEnumerable<RoomPopularityDto>>(popularity));
    }

    /// <summary>
    /// Завантаженість залів за період
    /// </summary>
    [HttpGet("load")]
    [ProducesResponseType(typeof(IEnumerable<RoomLoadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoomLoad([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var load = await _reportManager.GetRoomLoadAsync(from, to);
        return Ok(_mapper.Map<IEnumerable<RoomLoadDto>>(load));
    }
}

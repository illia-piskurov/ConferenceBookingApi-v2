using ConferenceBookingApi.DTOs.Reports;
using ConferenceBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("revenue")]
    [ProducesResponseType(typeof(RevenueReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenue([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var report = await _reportService.GetRevenueReportAsync(from, to);
        return Ok(report);
    }

    [HttpGet("popularity")]
    [ProducesResponseType(typeof(IEnumerable<RoomPopularityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularity()
    {
        var report = await _reportService.GetRoomPopularityAsync();
        return Ok(report);
    }

    [HttpGet("load")]
    [ProducesResponseType(typeof(IEnumerable<RoomLoadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoad([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var report = await _reportService.GetRoomLoadAsync(from, to);
        return Ok(report);
    }
}

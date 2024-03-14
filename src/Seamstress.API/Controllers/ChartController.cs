using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class ChartController : ControllerBase
  {
    private readonly IChartService _chartService;

    public ChartController(IChartService chartService)
    {
      this._chartService = chartService;
    }

    [HttpGet("pie")]
    public async Task<IActionResult> GetPieChart([FromQuery] string data, DateTime periodBegin, DateTime periodEnd)
    {
      try
      {
        PieChart chartData = await this._chartService.GetPieChartAsync(data, periodBegin, periodEnd);
        if (chartData == null) return NoContent();

        return Ok(chartData);
      }
      catch (Exception ex)
      {
        return BadRequest($"Não foi possível recuperar os dados. Erro: {ex.Message}");
      }
    }
  }
}
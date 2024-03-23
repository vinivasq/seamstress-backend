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

    [HttpGet("doughnut")]
    public async Task<IActionResult> GetDoughnutChart([FromQuery] string data, DateTime periodBegin, DateTime periodEnd)
    {
      try
      {
        DoughnutChart chartData = await this._chartService.GetDoughnutChartAsync(
          data,
          DateOnly.FromDateTime(periodBegin),
          DateOnly.FromDateTime(periodEnd)
        );
        if (chartData == null || chartData.DataSets.Count == 0) return NoContent();

        return Ok(chartData);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível recuperar os dados. Erro: {ex.Message}");
      }
    }

    [HttpGet("BarLine")]
    public async Task<IActionResult> GetBarLineChart([FromQuery] string data, DateTime periodBegin, DateTime periodEnd)
    {
      try
      {
        BarLineChart barLineChart = await this._chartService.GetBarLineChartAsync(
          data,
          DateOnly.FromDateTime(periodBegin),
          DateOnly.FromDateTime(periodEnd)
        );
        if (barLineChart == null || barLineChart.DataSets.Count == 0) return NoContent();

        return Ok(barLineChart);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível recuperar os dados. Erro: {ex.Message}");
      }
    }
  }
}
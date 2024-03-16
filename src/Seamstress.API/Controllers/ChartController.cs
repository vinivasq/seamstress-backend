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
        DoughnutChart chartData = await this._chartService.GetDoughnutChartAsync(data, periodBegin, periodEnd);
        if (chartData == null || chartData.DataSets.Count == 0) return NoContent();

        return Ok(chartData);
      }
      catch (Exception ex)
      {
        return BadRequest($"Não foi possível recuperar os dados. Erro: {ex.Message}");
      }
    }
  }
}
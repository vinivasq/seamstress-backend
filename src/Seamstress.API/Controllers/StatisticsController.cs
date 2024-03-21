using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class StatisticsController : ControllerBase
  {
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
      this._statisticsService = statisticsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatistics()
    {
      try
      {
        var statistics = await this._statisticsService.GetStatisticsAsync();
        if (statistics is null || statistics.Length == 0) return NoContent();

        return Ok(statistics);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar estatísticas. Erro: {ex.Message}");
      }
    }
  }
}
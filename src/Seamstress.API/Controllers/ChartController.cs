using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.DTO;
using Seamstress.ViewModel;

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
        DoughnutChart chartData = new() { };

        if (data.Trim().ToLower() == "region")
          chartData = await this._chartService.GetRegionDoughnutChartAsync(
            DateOnly.FromDateTime(periodBegin),
            DateOnly.FromDateTime(periodEnd)
          );
        else if (data.Trim().ToLower() == "model")
          chartData = await this._chartService.GetModelDoughnutChartAsync(
            DateOnly.FromDateTime(periodBegin),
            DateOnly.FromDateTime(periodEnd)
          );
        else
          return BadRequest("Tipo de dado inválido");

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
        if (data.Trim().ToLower() == "orders")
        {
          BarLineChartDto chartData = await this._chartService.GetOrderBarLineChartAsync(
              DateOnly.FromDateTime(periodBegin),
              DateOnly.FromDateTime(periodEnd)
            );
          if (chartData == null || chartData.DataSets.Count == 0) return NoContent();
          return Ok(chartData);
        }
        else if (data.Trim().ToLower() == "revenue")
        {
          RevenueBarLineChartDto chartData = await this._chartService.GetRevenueBarLineChartAsync(
              DateOnly.FromDateTime(periodBegin),
              DateOnly.FromDateTime(periodEnd)
            );
          if (chartData == null || chartData.DataSets.Count == 0) return NoContent();
          return Ok(chartData);
        }
        return BadRequest("Tipo de dado inválido");
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível recuperar os dados. Erro: {ex.Message}");
      }
    }
  }
}
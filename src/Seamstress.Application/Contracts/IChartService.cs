using Seamstress.Application.Dtos;
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Application.Contracts
{
  public interface IChartService
  {
    public Task<DoughnutChart> GetRegionDoughnutChartAsync(DateOnly periodBegin, DateOnly periodEnd);
    public Task<DoughnutChart> GetModelDoughnutChartAsync(DateOnly periodBegin, DateOnly periodEnd);
    public Task<BarLineChartDto> GetOrderBarLineChartAsync(DateOnly periodBegin, DateOnly periodEnd);
    public Task<RevenueBarLineChartDto> GetRevenueBarLineChartAsync(DateOnly periodBegin, DateOnly periodEnd);
  }
}
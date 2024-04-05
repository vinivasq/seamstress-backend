using Seamstress.Application.Dtos;
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Application.Contracts
{
  public interface IChartService
  {
    public Task<DoughnutChart> GetDoughnutChartAsync(string data, DateOnly periodBegin, DateOnly periodEnd);
    public Task<BarLineChartDto> GetBarLineChartAsync(string data, DateOnly periodBegin, DateOnly periodEnd);
  }
}
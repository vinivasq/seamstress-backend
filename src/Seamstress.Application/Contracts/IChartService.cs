using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Application.Contracts
{
  public interface IChartService
  {
    public Task<DoughnutChart> GetDoughnutChartAsync(string data, DateOnly periodBegin, DateOnly periodEnd);
  }
}
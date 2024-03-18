using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Persistence.Contracts
{
  public interface IChartPersistence
  {
    public Task<DoughnutChart> GetRegionDoughnutChartAsync(DateOnly periodBegin, DateOnly periodEnd);
    public Task<DoughnutChart> GetModelDoughnutChartAsync(DateOnly periodBegin, DateOnly periodEnd);
  }
}
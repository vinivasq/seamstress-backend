using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Persistence.Contracts
{
  public interface IChartPersistence
  {
    public Task<DoughnutChart> GetRegionDoughnutChartAsync(DateTime periodBegin, DateTime periodEnd);
    public Task<DoughnutChart> GetModelDoughnutChartAsync(DateTime periodBegin, DateTime periodEnd);
  }
}
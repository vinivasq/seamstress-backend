using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Persistence.Contracts
{
  public interface IChartPersistence
  {
    public Task<PieChart> GetRegionPieChartAsync(DateTime periodBegin, DateTime periodEnd);
    public Task<PieChart> GetModelPieChartAsync(DateTime periodBegin, DateTime periodEnd);
  }
}
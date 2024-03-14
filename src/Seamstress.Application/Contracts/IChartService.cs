using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Application.Contracts
{
  public interface IChartService
  {
    public Task<PieChart> GetPieChartAsync(string data, DateTime periodBegin, DateTime periodEnd);
  }
}
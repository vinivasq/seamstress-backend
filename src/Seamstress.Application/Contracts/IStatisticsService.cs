using Seamstress.ViewModel;

namespace Seamstress.Application.Contracts
{
  public interface IStatisticsService
  {
    public Task<Statistic[]> GetStatisticsAsync();
  }
}
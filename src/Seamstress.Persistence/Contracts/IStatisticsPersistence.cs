using Seamstress.ViewModel;

namespace Seamstress.Persistence.Contracts
{
  public interface IStatisticsPersistence
  {
    public Task<Statistic[]> GetStatisticsAsync();
  }
}
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Persistence.Contracts
{
  public interface IStatisticsPersistence
  {
    public Task<Statistic[]> GetStatisticsAsync();
  }
}
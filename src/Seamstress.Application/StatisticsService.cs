using Seamstress.Application.Contracts;
using Seamstress.Persistence.Contracts;
using Seamstress.ViewModel;

namespace Seamstress.Application
{
  public class StatisticsService : IStatisticsService
  {
    private readonly IStatisticsPersistence _statisticsPersistence;

    public StatisticsService(IStatisticsPersistence statisticsPersistence)
    {
      this._statisticsPersistence = statisticsPersistence;
    }
    public async Task<Statistic[]> GetStatisticsAsync()
    {
      try
      {
        return await this._statisticsPersistence.GetStatisticsAsync();

      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
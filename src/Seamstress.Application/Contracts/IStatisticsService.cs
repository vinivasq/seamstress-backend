using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Application.Contracts
{
  public interface IStatisticsService
  {
    public Task<Statistic[]> GetStatisticsAsync();
  }
}
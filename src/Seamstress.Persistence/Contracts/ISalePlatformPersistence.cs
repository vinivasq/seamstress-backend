using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface ISalePlatformPersistence
  {
    public Task<SalePlatform[]> GetSalePlatformsAsync();
  }
}
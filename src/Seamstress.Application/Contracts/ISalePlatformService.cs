using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface ISalePlatformService
  {
    public Task<SalePlatform[]> GetSalesPlatformAsync();
  }
}
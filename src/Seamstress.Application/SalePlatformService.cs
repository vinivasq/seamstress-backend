using Seamstress.Application.Contracts;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class SalePlatformService : ISalePlatformService
  {
    private readonly ISalePlatformPersistence _salePlatformPersistence;

    public SalePlatformService(ISalePlatformPersistence salePlatformPersistence)
    {
      this._salePlatformPersistence = salePlatformPersistence;
    }
    public async Task<SalePlatform[]> GetSalesPlatformAsync()
    {
      try
      {
        return await this._salePlatformPersistence.GetSalePlatformsAsync();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
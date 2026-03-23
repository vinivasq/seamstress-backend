using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IImportMappingPersistence
  {
    Task<ImportMapping?> GetBySalePlatformIdAsync(int salePlatformId);
  }
}

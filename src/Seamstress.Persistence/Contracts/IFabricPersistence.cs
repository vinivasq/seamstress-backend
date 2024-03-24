using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IFabricPersistence
  {
    Task<Fabric[]> GetAllFabricsAsync();
    Task<bool> CheckFKAsync(int id);
  }
}
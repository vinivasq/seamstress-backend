using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface ISetPersistence
  {
    Task<Set[]> GetAllSetsAsync();
    Task<Set> GetSetByIdAsync(int id);
  }
}
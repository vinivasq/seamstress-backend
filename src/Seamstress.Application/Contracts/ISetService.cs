using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface ISetService
  {
    public Task<Set> AddSet(Set model);
    public Task<Set> UpdateSet(int id, Set model);
    public Task<bool> DeleteSet(int id);

    public Task<Set[]> GetSetsAsync();
    public Task<Set> GetSetByIdAsync(int id);
  }
}
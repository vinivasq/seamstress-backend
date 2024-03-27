using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface ISetService
  {
    public Task<Set> AddSet(Set model);
    public Task<Set> UpdateSet(int id, Set model);
    public Task<Set> SetActiveState(int id, bool state);
    public Task<bool> CheckFK(int id);
    public Task<bool> DeleteSet(int id);

    public Task<Set[]> GetSetsAsync();
    public Task<Set> GetSetByIdAsync(int id);
  }
}
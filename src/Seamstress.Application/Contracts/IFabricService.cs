
using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface IFabricService
  {
    public Task<Fabric> AddFabric(Fabric model);
    public Task<Fabric> UpdateFabric(int id, Fabric model);
    public Task<Fabric> SetActiveState(int id, bool state);
    public Task<bool> CheckFK(int id);
    public Task<bool> DeleteFabric(int id);


    public Task<Fabric[]> GetFabricsAsync();
    public Task<Fabric> GetFabricByIdAsync(int id);

  }
}
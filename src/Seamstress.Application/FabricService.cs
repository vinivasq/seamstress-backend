using Seamstress.Application.Contracts;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class FabricService : IFabricService
  {
    private readonly IFabricPersistence _fabricPersistence;
    private readonly IGeneralPersistence _generalPersistence;

    public FabricService(IFabricPersistence fabricPersistence, IGeneralPersistence generalPersistence)
    {
      this._generalPersistence = generalPersistence;
      this._fabricPersistence = fabricPersistence;
    }

    public async Task<Fabric> AddFabric(Fabric model)
    {
      try
      {
        _generalPersistence.Add<Fabric>(model);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _fabricPersistence.GetFabricByIdAsync(model.Id);
        }

        throw new Exception("Não foi possível cadastrar o tecido.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Fabric> UpdateFabric(int id, Fabric model)
    {
      try
      {
        var fabric = await _fabricPersistence.GetFabricByIdAsync(id);
        if (fabric == null) throw new Exception("Não foi possível encontrar o tecido a ser atualizada");

        model.Id = fabric.Id;

        _generalPersistence.Update<Fabric>(model);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _fabricPersistence.GetFabricByIdAsync(model.Id);
        }

        throw new Exception("Não foi possível atualizar o tecido.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> DeleteFabric(int id)
    {
      try
      {
        var fabric = await _fabricPersistence.GetFabricByIdAsync(id);
        if (fabric == null) throw new Exception("Não foi possível encontrar o tecido a ser deletada.");

        _generalPersistence.Delete<Fabric>(fabric);

        return await _generalPersistence.SaveChangesAsync();
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Fabric[]> GetFabricsAsync()
    {
      try
      {
        return await _fabricPersistence.GetAllFabricsAsync();
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Fabric> GetFabricByIdAsync(int id)
    {
      try
      {
        var fabric = await _fabricPersistence.GetFabricByIdAsync(id);
        if (fabric == null) throw new Exception("Nâo foi possível encontrar o tecido informada.");

        return fabric;
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}
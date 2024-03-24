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
          return await _fabricPersistence.GetFabricByIdAsync(model.Id)
            ?? throw new Exception("Não foi possível encontrar o tecido após o cadastro.");
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
        var fabric = await _fabricPersistence.GetFabricByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o tecido a ser atualizado");
        model.Id = fabric.Id;

        _generalPersistence.Update<Fabric>(model);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _fabricPersistence.GetFabricByIdAsync(model.Id)
            ?? throw new Exception("Não foi possível encontrar o tecido após atualização.");
        }

        throw new Exception("Não foi possível atualizar o tecido.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Fabric> SetActiveState(int id, bool state)
    {
      try
      {
        var fabric = await _fabricPersistence.GetFabricByIdAsync(id)
          ?? throw new Exception("Nâo foi possível encontrar o tecido informado.");

        fabric.IsActive = state;

        _generalPersistence.Update(fabric);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _fabricPersistence.GetFabricByIdAsync(fabric.Id)
            ?? throw new Exception("Não foi possível listar a cor após atualização.");
        }

        throw new Exception("Não foi possível atualizar a cor.");
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
        var fabric = await _fabricPersistence.GetFabricByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o tecido a ser deletado.");

        if (await _fabricPersistence.CheckFKAsync(id) == false)
        {
          _generalPersistence.Delete<Fabric>(fabric);
          return await _generalPersistence.SaveChangesAsync();
        }

        throw new Exception("Não é possível deletar pois possuem registros vinculados");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> CheckFK(int id)
    {
      try
      {
        var color = await _fabricPersistence.GetFabricByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o tecido a ser validado.");

        return await _fabricPersistence.CheckFKAsync(id);
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
        var fabric = await _fabricPersistence.GetFabricByIdAsync(id)
          ?? throw new Exception("Nâo foi possível encontrar o tecido informado.");
        return fabric;
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}
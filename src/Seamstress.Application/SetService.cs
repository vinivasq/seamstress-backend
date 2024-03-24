using Seamstress.Application.Contracts;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class SetService : ISetService
  {
    private readonly ISetPersistence _setPersistence;
    private readonly IGeneralPersistence _generalPersistence;

    public SetService(IGeneralPersistence generalPersistence, ISetPersistence setPersistence)
    {
      this._generalPersistence = generalPersistence;
      this._setPersistence = setPersistence;
    }

    public async Task<Set> AddSet(Set model)
    {
      try
      {
        _generalPersistence.Add<Set>(model);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await this._setPersistence.GetSetByIdAsync(model.Id)
            ?? throw new Exception("Não foi possível listar o conjunto após o cadastro");
        }

        throw new Exception("Não foi possível criar o conjunto.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
    public async Task<Set> UpdateSet(int id, Set model)
    {
      try
      {
        var set = await _setPersistence.GetSetByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o conjunto a ser atualizado.");
        model.Id = set.Id;

        _generalPersistence.Update<Set>(model);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _setPersistence.GetSetByIdAsync(model.Id)
            ?? throw new Exception("Não foi possível listar o conjunto após atualização.");
        }

        throw new Exception("Não foi possível atualizar o conjunto.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
    public async Task<Set> SetActiveState(int id, bool state)
    {
      try
      {
        var set = await _setPersistence.GetSetByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o conjunto informado.");

        set.IsActive = state;

        _generalPersistence.Update(set);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _setPersistence.GetSetByIdAsync(set.Id)
            ?? throw new Exception("Não foi possível listar o conjunto após atualização.");
        }

        throw new Exception("Não foi possível atualizar o conjunto.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
    public async Task<bool> DeleteSet(int id)
    {
      try
      {
        var set = await _setPersistence.GetSetByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o conjunto a ser deletado.");

        if (await _setPersistence.CheckFKAsync(id) == false)
        {
          _generalPersistence.Delete<Set>(set);
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
        var set = await _setPersistence.GetSetByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o conjunto a ser validado.");

        return await _setPersistence.CheckFKAsync(id);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
    public async Task<Set[]> GetSetsAsync()
    {
      try
      {
        return await _setPersistence.GetAllSetsAsync();
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
    public async Task<Set> GetSetByIdAsync(int id)
    {
      try
      {
        return await _setPersistence.GetSetByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o conjunto desejado.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}
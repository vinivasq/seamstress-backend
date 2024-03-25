using Seamstress.Application.Contracts;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class SizeService : ISizeService
  {
    private readonly ISizePersistence _sizePersistence;
    private readonly IGeneralPersistence _generalPersistence;

    public SizeService(ISizePersistence sizePersistence, IGeneralPersistence generalPersistence)
    {
      this._generalPersistence = generalPersistence;
      this._sizePersistence = sizePersistence;
    }

    public async Task<Size> AddSize(Size model)
    {
      try
      {
        _generalPersistence.Add<Size>(model);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _sizePersistence.GetSizeByIdAsync(model.Id)
            ?? throw new Exception("Não foi possível listar o tamanho após o cadastro.");
        }

        throw new Exception("Não foi possível cadastrar o tamanho.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Size> SetActiveState(int id, bool state)
    {
      try
      {
        var size = await _sizePersistence.GetSizeByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o tamanho informado.");

        size.IsActive = state;

        _generalPersistence.Update(size);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _sizePersistence.GetSizeByIdAsync(size.Id)
            ?? throw new Exception("Não foi possível listar o tamanho após atualização.");
        }

        throw new Exception("Não foi possível atualizar o tamanho.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> DeleteSize(int id)
    {
      try
      {
        var size = await _sizePersistence.GetSizeByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o tamanho a ser deletado.");

        if (await _sizePersistence.CheckFKAsync(id) == false)
        {
          _generalPersistence.Delete<Size>(size);
          return await _generalPersistence.SaveChangesAsync();
        }

        throw new Exception("Não é possível deletar pois existem registros vinculados");
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
        var set = await _sizePersistence.GetSizeByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o tamanho a ser validado.");

        return await _sizePersistence.CheckFKAsync(id);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<Size[]> GetSizesAsync()
    {
      try
      {
        return await _sizePersistence.GetAllSizesAsync();
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Size> GetSizeByIdAsync(int id)
    {
      try
      {
        return await _sizePersistence.GetSizeByIdAsync(id)
          ?? throw new Exception("Nâo foi possível encontrar o tamanho informado.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}
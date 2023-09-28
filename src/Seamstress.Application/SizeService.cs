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
          return await _sizePersistence.GetSizeByIdAsync(model.Id);
        }

        throw new Exception("Não foi possível cadastrar o tamanho.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Size> UpdateSize(int id, Size model)
    {
      try
      {
        var size = await _sizePersistence.GetSizeByIdAsync(id);
        if (size == null) throw new Exception("Não foi possível encontrar a tamanho a ser atualizado");

        model.Id = size.Id;

        _generalPersistence.Update<Size>(model);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _sizePersistence.GetSizeByIdAsync(model.Id);
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
        var size = await _sizePersistence.GetSizeByIdAsync(id);
        if (size == null) throw new Exception("Não foi possível encontrar o tamanho a ser deletado.");

        _generalPersistence.Delete<Size>(size);

        return await _generalPersistence.SaveChangesAsync();
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
        var size = await _sizePersistence.GetSizeByIdAsync(id);
        if (size == null) throw new Exception("Nâo foi possível encontrar o tamanho informado.");

        return size;
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}
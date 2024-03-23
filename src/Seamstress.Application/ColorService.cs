using Seamstress.Application.Contracts;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class ColorService : IColorService
  {
    private readonly IColorPersistence _colorPersistence;
    private readonly IGeneralPersistence _generalPersistence;

    public ColorService(IColorPersistence colorPersistence, IGeneralPersistence generalPersistence)
    {
      this._generalPersistence = generalPersistence;
      this._colorPersistence = colorPersistence;
    }

    public async Task<Color> AddColor(Color model)
    {
      try
      {
        _generalPersistence.Add<Color>(model);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _colorPersistence.GetColorByIdAsync(model.Id);
        }

        throw new Exception("Não foi possível cadastrar a cor.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Color> UpdateColor(int id, Color model)
    {
      try
      {
        var color = await _colorPersistence.GetColorByIdAsync(id);
        if (color == null) throw new Exception("Não foi possível encontrar a cor a ser atualizada");

        model.Id = color.Id;

        _generalPersistence.Update<Color>(model);

        if (await _generalPersistence.SaveChangesAsync())
        {
          return await _colorPersistence.GetColorByIdAsync(model.Id);
        }

        throw new Exception("Não foi possível atualizar a cor.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> DeleteColor(int id)
    {
      try
      {
        var color = await _colorPersistence.GetColorByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar a cor a ser deletada.");


        if (await _colorPersistence.CheckFKAsync(id) == false)
        {
          _generalPersistence.Delete<Color>(color);
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
        var color = await _colorPersistence.GetColorByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar a cor a ser validada.");

        return await this._colorPersistence.CheckFKAsync(id);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<Color[]> GetColorsAsync()
    {
      try
      {
        return await _colorPersistence.GetAllColorsAsync();
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Color> GetColorByIdAsync(int id)
    {
      try
      {
        var color = await _colorPersistence.GetColorByIdAsync(id);
        if (color == null) throw new Exception("Nâo foi possível encontrar a cor informada.");

        return color;
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}
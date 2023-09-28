using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IColorPersistence
  {
    Task<Color[]> GetAllColorsAsync();
    Task<Color> GetColorByIdAsync(int id);
  }
}
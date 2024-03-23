using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IColorPersistence
  {
    Task<Color[]> GetAllColorsAsync();
    Task<bool> CheckFKAsync(int id);
  }
}
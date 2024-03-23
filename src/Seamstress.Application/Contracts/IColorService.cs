
using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface IColorService
  {
    public Task<Color> AddColor(Color model);
    public Task<Color> UpdateColor(int id, Color model);
    public Task<bool> CheckFK(int id);
    public Task<bool> DeleteColor(int id);


    public Task<Color[]> GetColorsAsync();
    public Task<Color> GetColorByIdAsync(int id);

  }
}
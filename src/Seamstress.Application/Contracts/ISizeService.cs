
using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface ISizeService
  {
    public Task<Size> AddSize(Size model);
    public Task<Size> SetActiveState(int id, bool state);
    public Task<bool> CheckFK(int id);
    public Task<bool> DeleteSize(int id);


    public Task<Size[]> GetSizesAsync();
    public Task<Size> GetSizeByIdAsync(int id);

  }
}
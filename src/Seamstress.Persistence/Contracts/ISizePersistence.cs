using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface ISizePersistence
  {
    Task<Size[]> GetAllSizesAsync();
    Task<Size> GetSizeByIdAsync(int id);
  }
}
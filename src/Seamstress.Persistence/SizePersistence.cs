using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class SizePersistence : ISizePersistence
  {
    private readonly SeamstressContext _context;

    public SizePersistence(SeamstressContext context)
    {
      this._context = context;

    }

    public async Task<Size[]> GetAllSizesAsync()
    {
      IQueryable<Size> query = _context.Sizes;

      return await query.AsNoTracking().ToArrayAsync();

    }

    public async Task<Size> GetSizeByIdAsync(int id)
    {
      IQueryable<Size> query = _context.Sizes.Where(c => c.Id == id);

      return await query.AsNoTracking().FirstAsync();
    }
  }
}
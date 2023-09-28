using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class ColorPersistence : IColorPersistence
  {
    private readonly SeamstressContext _context;

    public ColorPersistence(SeamstressContext context)
    {
      this._context = context;

    }

    public async Task<Color[]> GetAllColorsAsync()
    {
      IQueryable<Color> query = _context.Colors;

      return await query.AsNoTracking().ToArrayAsync();

    }

    public async Task<Color> GetColorByIdAsync(int id)
    {
      IQueryable<Color> query = _context.Colors.Where(c => c.Id == id);

      return await query.AsNoTracking().FirstAsync();
    }
  }
}
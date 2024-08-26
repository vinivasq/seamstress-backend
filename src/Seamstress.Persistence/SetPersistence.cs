using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class SetPersistence : ISetPersistence
  {
    private readonly SeamstressContext _context;
    public SetPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public async Task<Set[]> GetAllSetsAsync()
    {
      IQueryable<Set> query = _context.Sets;

      return await query.AsNoTracking().ToArrayAsync();
    }

    public async Task<Set?> GetSetByIdAsync(int id)
    {
      IQueryable<Set> query = _context.Sets.Where(set => set.Id == id);
      query = query.Include(set => set.SetItems);

      return await query.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<bool> CheckFKAsync(int id)
    {
      return await this._context.Items.AnyAsync(x => x.SetItem != null && x.SetItem.SetId == id);
    }
  }
}
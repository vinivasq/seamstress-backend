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

    public async Task<Set> GetSetByIdAsync(int id)
    {
      IQueryable<Set> query = _context.Sets.Where(set => set.Id == id);

      return await query.AsNoTracking().FirstAsync();
    }
  }
}
using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class FabricPersistence : IFabricPersistence
  {
    private readonly SeamstressContext _context;

    public FabricPersistence(SeamstressContext context)
    {
      this._context = context;

    }

    public async Task<Fabric[]> GetAllFabricsAsync()
    {
      IQueryable<Fabric> query = _context.Fabrics;

      return await query.AsNoTracking().ToArrayAsync();

    }

    public async Task<Fabric> GetFabricByIdAsync(int id)
    {
      IQueryable<Fabric> query = _context.Fabrics.Where(c => c.Id == id);

      return await query.AsNoTracking().FirstAsync();
    }
  }
}
using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class SalePlatformPersistence : ISalePlatformPersistence
  {
    private readonly SeamstressContext _context;

    public SalePlatformPersistence(SeamstressContext context)
    {
      this._context = context;
    }
    public async Task<SalePlatform[]> GetSalePlatformsAsync()
    {
      IQueryable<SalePlatform> query = this._context.SalePlatforms
        .Where(platform => platform.Id != 99)
        .OrderBy(platform => platform.Id);

      return await query.AsNoTracking().ToArrayAsync();
    }
  }
}
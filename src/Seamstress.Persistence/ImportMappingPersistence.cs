using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;
using Seamstress.Persistence.Context;

namespace Seamstress.Persistence
{
  public class ImportMappingPersistence : IImportMappingPersistence
  {
    private readonly SeamstressContext _context;

    public ImportMappingPersistence(SeamstressContext context)
    {
      _context = context;
    }

    public async Task<ImportMapping?> GetBySalePlatformIdAsync(int salePlatformId)
    {
      return await _context.ImportMappings
          .Include(m => m.SalePlatform)
          .FirstOrDefaultAsync(m => m.SalePlatformId == salePlatformId);
    }
  }
}

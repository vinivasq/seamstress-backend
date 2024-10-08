using Microsoft.EntityFrameworkCore;
using Seamstress.Persistence.Context;

namespace Seamstress.API.Helpers
{
  public class MigrationsHelper
  {
    public static async Task ManageDataAsync(IServiceProvider svcProvider)
    {
      //Service: An instance of db context
      var dbContextSvc = svcProvider.GetRequiredService<SeamstressContext>();

      //Migration: This is the programmatic equivalent to Update-Database
      await dbContextSvc.Database.MigrateAsync();
    }
  }
}
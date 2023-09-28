using Microsoft.EntityFrameworkCore;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class GeneralPersistence : IGeneralPersistence
  {
    private readonly SeamstressContext _context;
    public GeneralPersistence(SeamstressContext context)
    {
      this._context = context;

    }

    public void Add<T>(T entity) where T : class
    {
      _context.Add(entity);
    }
    public void Update<T>(T entity) where T : class
    {
      _context.Entry(entity).State = EntityState.Modified;
      _context.Set<T>().Update(entity);
    }
    public void Delete<T>(T entity) where T : class
    {
      _context.Remove(entity);
    }
    public void DeleteRange<T>(T[] entityArray) where T : class
    {
      _context.RemoveRange(entityArray);
    }
    public async Task<bool> SaveChangesAsync()
    {
      return (await _context.SaveChangesAsync()) > 0;
    }
    public bool SaveChanges()
    {
      return _context.SaveChanges() > 0;
    }
  }
}
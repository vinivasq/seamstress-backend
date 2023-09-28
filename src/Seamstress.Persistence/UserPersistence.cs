using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Seamstress.Persistence
{
  public class UserPersistence : IUserPersistence
  {
    private readonly SeamstressContext _context;

    public UserPersistence(SeamstressContext context)
    {
      this._context = context;

    }

    public async Task<User[]> GetAllUsersAsync()
    {
      IQueryable<User> query = _context.Users;

      query.OrderBy(user => user.Id);

      return await query.AsNoTracking().ToArrayAsync();
    }
    public async Task<User> GetUserByIdAsync(int id)
    {
      IQueryable<User> query = _context.Users;

      query.Where(user => user.Id == id);

      return await query.AsNoTracking().FirstAsync();
    }
    public async Task<User[]> GetAllUsersByNameAsync(string name)
    {
      IQueryable<User> query = _context.Users;

      query.OrderBy(user => user.Name).Where(user => user.Name.ToLower() == name.ToLower());

      return await query.AsNoTracking().ToArrayAsync();
    }
  }
}
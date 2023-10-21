using Microsoft.EntityFrameworkCore;
using Seamstress.Domain.Identity;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class UserPersistence : IUserPersistence
  {
    private readonly SeamstressContext _context;

    public UserPersistence(SeamstressContext context)
    {
      this._context = context;

    }

    public async Task<User[]> GetAllExecutorsAsync()
    {
      return await _context.Users.Where(user => user.Role == Domain.Enum.Roles.Executor).AsNoTracking().ToArrayAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
      return await _context.Users.AsNoTracking().FirstAsync(user => user.Id == id);
    }

    public async Task<User> GetUserByUserNameAsync(string username)
    {
      return await _context.Users.AsNoTracking().FirstAsync(user => user.UserName == username.ToLower());
    }

  }
}
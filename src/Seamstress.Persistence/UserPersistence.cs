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
      return await _context.Users.Where(user => user.Role == Domain.Enum.Roles.Executor && user.IsActive == true).AsNoTracking().ToArrayAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
      User query = await _context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id)
        ?? throw new Exception($"Não foi encontrado um usuário de Id: {id}");

      return query;
    }

    public async Task<User> GetUserByUserNameAsync(string username)
    {
      User query = await _context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.UserName == username.ToLower())
        ?? throw new Exception($"Não foi encontrado um usuário de com o nomde de usuário: {username}");

      return query;
    }

  }
}
using Seamstress.Domain.Identity;

namespace Seamstress.Persistence.Contracts
{
  public interface IUserPersistence
  {
    Task<User[]> GetAllExecutorsAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<User> GetUserByUserNameAsync(string username);

  }
}
using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IUserPersistence
  {
    Task<User[]> GetAllUsersByNameAsync(string name);
    Task<User[]> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
  }
}
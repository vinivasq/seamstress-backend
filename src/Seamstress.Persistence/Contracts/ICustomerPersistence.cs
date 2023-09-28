using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface ICustomerPersistence
  {
    Task<Customer[]> GetCustomersAsync();
    Task<Customer> GetCustomerByIdAsync(int id);
  }
}
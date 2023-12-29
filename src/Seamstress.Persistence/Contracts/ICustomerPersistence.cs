using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface ICustomerPersistence
  {
    Task<Customer[]> GetCustomersAsync(string term);
    Task<Customer> GetCustomerByIdAsync(int id);
  }
}
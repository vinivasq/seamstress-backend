using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface ICustomerPersistence
  {
    Task<Customer> GetCustomerByPKAsync(string CPF_CNPJ);
    Task<Customer> GetCustomerByIdAsync(int id);
  }
}
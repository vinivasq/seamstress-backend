using Seamstress.Domain;
using Seamstress.Persistence.Helpers;

namespace Seamstress.Persistence.Contracts
{
  public interface ICustomerPersistence
  {
    Task<PageList<Customer>> GetCustomersAsync(PageParams pageParams);
    Task<Customer?> GetCustomerByPKAsync(string CPF_CNPJ);
    Task<Customer?> GetCustomerByIdAsync(int id);
    Task<bool> CheckFKAsync(int id);

  }
}
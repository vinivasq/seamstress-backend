using Seamstress.Domain;
using Seamstress.Persistence.Dtos;
using Seamstress.Persistence.Helpers;
using Seamstress.Persistence.Parameters;

namespace Seamstress.Persistence.Contracts
{
  public interface ICustomerPersistence
  {
    Task<PageList<Customer>> GetCustomersAsync(PageParams pageParams);
    Task<Customer?> GetCustomerByPKAsync(string CPF_CNPJ);
    Task<Customer?> GetCustomerByIdAsync(int id);
    Task<bool> CheckFKAsync(int id);
    Task<List<CustomerExportDto>> GetCustomersForExportAsync(CustomerExportParams exportParams);
  }
}
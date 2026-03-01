using Seamstress.Application.Dtos;
using Seamstress.Persistence.Helpers;
using Seamstress.Persistence.Parameters;

namespace Seamstress.Application.Contracts
{
  public interface ICustomerService
  {
    public Task<CustomerDto> AddCustomer(CustomerDto model);
    public Task<CustomerDto> UpdateCustomer(int id, CustomerDto model);
    public Task<CustomerDto> SetActiveState(int id, bool state);
    public Task<bool> CheckFK(int id);
    public Task<bool> DeleteCustomer(int id);

    public Task<PageList<CustomerDto>> GetCustomersAsync(PageParams pageParams);
    public Task<CustomerDto> GetCustomerByIdAsync(int id);
    public Task<List<CustomerExportDto>> GetCustomersForExportAsync(CustomerExportParams exportParams);
  }
}
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Persistence.Helpers;

namespace Seamstress.Application.Contracts
{
  public interface ICustomerService
  {
    public Task<CustomerDto> AddCustomer(CustomerDto model);
    public Task<CustomerDto> UpdateCustomer(int id, CustomerDto model);
    public Task<bool> DeleteCustomer(int id);


    public Task<PageList<CustomerDto>> GetCustomersAsync(PageParams pageParams);
    public Task<CustomerDto> GetCustomerByIdAsync(int id);
  }
}
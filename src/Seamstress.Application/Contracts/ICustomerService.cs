using Seamstress.Application.Dtos;

namespace Seamstress.Application.Contracts
{
  public interface ICustomerService
  {
    public Task<CustomerDto> AddCustomer(CustomerDto model);
    public Task<CustomerDto> UpdateCustomer(int id, CustomerDto model);
    public Task<bool> DeleteCustomer(int id);


    public Task<CustomerDto[]> GetCustomersAsync();
    public Task<CustomerDto> GetCustomerByIdAsync(int id);
  }
}
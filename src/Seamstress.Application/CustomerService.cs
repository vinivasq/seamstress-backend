using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class CustomerService : ICustomerService
  {

    private readonly ICustomerPersistence _customerPersistence;
    private readonly IGeneralPersistence _generalPersistence;
    private readonly IMapper _mapper;

    public CustomerService(IGeneralPersistence generalPersistence,
                            ICustomerPersistence customerPersistence,
                            IMapper mapper
                          )
    {
      this._generalPersistence = generalPersistence;
      this._customerPersistence = customerPersistence;
      this._mapper = mapper;
    }

    public async Task<CustomerDto> AddCustomer(CustomerDto model)
    {
      try
      {
        var customers = await _customerPersistence.GetCustomersAsync("");
        if (customers.Where(x => x.CPF_CNPJ == model.CPF_CNPJ).FirstOrDefault() != null)
          throw new Exception("Cliente com CPF/CNPJ já existente");

        var customer = _mapper.Map<Customer>(model);
        _generalPersistence.Add<Customer>(customer);

        if (await _generalPersistence.SaveChangesAsync())
        {
          var customerResponse = await _customerPersistence.GetCustomerByIdAsync(customer.Id);

          return _mapper.Map<CustomerDto>(customerResponse);
        }

        throw new Exception("Não foi possível cadastrar o cliente");

      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<CustomerDto> UpdateCustomer(int id, CustomerDto model)
    {
      try
      {
        var customer = await _customerPersistence.GetCustomerByIdAsync(id) ?? throw new Exception("Não foi possível encontrar o cliente");
        model.Id = customer.Id;

        if (model.Sizings != null)
        {
          if (model.Sizings.Id == 0)
          {
            _generalPersistence.Add<Sizings>(model.Sizings);
          }
          else
          {
            _generalPersistence.Update<Sizings>(model.Sizings);
          }
        }

        _mapper.Map(model, customer);
        _generalPersistence.Update<Customer>(customer);

        if (await _generalPersistence.SaveChangesAsync())
        {
          var customerResponse = await _customerPersistence.GetCustomerByIdAsync(customer.Id);

          return _mapper.Map<CustomerDto>(customerResponse);
        }

        throw new Exception("Não foi possível atualizar o cliente");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> DeleteCustomer(int id)
    {
      try
      {
        var customer = await _customerPersistence.GetCustomerByIdAsync(id) ?? throw new Exception("Não foi posível encontrar o cliente");
        _generalPersistence.Delete(customer);

        return await _generalPersistence.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<CustomerDto[]> GetCustomersAsync(string term)
    {
      try
      {
        var customers = await _customerPersistence.GetCustomersAsync(term);

        return _mapper.Map<CustomerDto[]>(customers);
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(int id)
    {
      try
      {
        var customer = await _customerPersistence.GetCustomerByIdAsync(id);

        return _mapper.Map<CustomerDto>(customer);
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}
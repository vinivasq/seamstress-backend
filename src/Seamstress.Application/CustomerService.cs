using System.ComponentModel;
using System.Net.Http.Json;
using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Application.ResponseModels;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class CustomerService : ICustomerService
  {

    private readonly ICustomerPersistence _customerPersistence;
    private readonly IGeneralPersistence _generalPersistence;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public CustomerService(IGeneralPersistence generalPersistence,
                            ICustomerPersistence customerPersistence,
                            IMapper mapper,
                            HttpClient httpClient
                          )
    {
      this._generalPersistence = generalPersistence;
      this._customerPersistence = customerPersistence;
      this._mapper = mapper;
      this._httpClient = httpClient;
    }

    public async Task<CustomerDto> AddCustomer(CustomerDto model)
    {
      try
      {
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

    public async Task<CustomerDto[]> GetCustomersAsync()
    {
      try
      {
        var customers = await _customerPersistence.GetCustomersAsync();

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

    public async Task<bool> AcertaEnderecos()
    {
      try
      {
        IEnumerable<Customer> query = await _customerPersistence.GetCustomersAsync();

        List<Customer> customers = query.ToList();

        foreach (Customer customer in customers)
        {
          HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"https://viacep.com.br/ws/{customer.Cep}/json/");
          ViacepResponse response = await httpResponseMessage.Content.ReadFromJsonAsync<ViacepResponse>()
            ?? throw new Exception($"Erro ao desserializar cep do cliente: {customer.Id}");

          if (httpResponseMessage.IsSuccessStatusCode)
          {
            if (response.erro != null)
            {
              customer.City = "Não encontrada";
              customer.Neighborhood = "Não encontrado";

              _generalPersistence.Update<Customer>(customer);
              continue;
            }

            customer.City = response.localidade == "" ? "Não encontrada" : response.localidade;
            customer.Neighborhood = response.bairro == "" ? "Não encontrado" : response.bairro;

            _generalPersistence.Update<Customer>(customer);
          }
          else throw new Exception($"Erro na requisição viacep: STATUS -> {httpResponseMessage.StatusCode}");

        }

        if (_generalPersistence.SaveChanges()) return true;
        return false;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
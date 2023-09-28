using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;

namespace Seamstress.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CustomerController : ControllerBase
  {
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
      this._customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      try
      {
        var customers = await _customerService.GetCustomersAsync();
        if (customers == null) return NoContent();

        return Ok(customers);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível retornar os clientes. Erro: {ex.Message}");
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      try
      {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null) return NoContent();

        return Ok(customer);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar o cliente. Erro: {ex.Message}");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post(CustomerDto model)
    {
      try
      {
        var customer = await _customerService.AddCustomer(model);
        if (customer == null) return BadRequest("Não foi possível criar o pedido");

        return Ok(customer);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível criar o cliente. Erro: {ex.Message}");
      }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, CustomerDto model)
    {
      try
      {
        var customer = await _customerService.UpdateCustomer(id, model);
        if (customer == null) return BadRequest("Não foi possível atualizar o cliente");

        return Ok(customer);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível atualizar o cliente. Erro: {ex.Message}");
      }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        return await _customerService.DeleteCustomer(id) ? Ok(new { message = "Deletado com sucesso" }) : throw new Exception("Não foi possível deletar o cliente");
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível deletar o cliente. Erro: {ex.Message}");
      }
    }

  }
}
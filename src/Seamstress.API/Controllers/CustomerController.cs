using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.API.Extensions;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Persistence.Models;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class CustomerController : ControllerBase
  {
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
      this._customerService = customerService;
    }

    [HttpGet()]
    public async Task<IActionResult> Get([FromQuery] PageParams pageParams)
    {
      try
      {
        var customers = await _customerService.GetCustomersAsync(pageParams);
        if (customers.Count == 0) return NoContent();

        Response.AddPagination(customers.CurrentPage, customers.PageSize, customers.TotalCount, customers.TotalPages);

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

    [HttpGet("fk/{id}")]
    public async Task<IActionResult> CheckFK(int id)
    {
      try
      {
        return Ok(await _customerService.CheckFK(id));
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
        if (customer == null) return BadRequest("Não foi possível criar o cliente");

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

    [HttpPatch("active/{id}")]
    public async Task<IActionResult> SetActiveState(int id, [FromQuery] bool state)
    {
      try
      {
        var customer = await _customerService.SetActiveState(id, state);
        if (customer == null) return BadRequest("Não foi possível atualizar o cliente");

        return Ok(customer);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível editar o cliente. Erro: {ex.Message}");
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
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;

namespace Seamstress.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{

  private readonly IOrderService _orderService;
  public OrderController(IOrderService orderService)
  {
    this._orderService = orderService;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    try
    {
      var orders = await _orderService.GetAllOrdersAsync();
      if (orders == null) return NoContent();

      return Ok(orders);
    }
    catch (Exception ex)
    {
      return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar pedidos. Erro: {ex.Message}");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    try
    {
      var order = await _orderService.GetOrderByIdAsync(id);
      if (order == null) return NoContent();

      return Ok(order);
    }
    catch (Exception ex)
    {
      return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar o pedido. Erro: {ex.Message}");
    }
  }

  [HttpPost()]
  public async Task<IActionResult> Post(OrderInputDto model)
  {
    try
    {
      var order = await _orderService.AddOrder(model);
      if (order == null) return BadRequest("Não foi possível criar o pedido");

      return Ok(order);
    }
    catch (Exception ex)
    {
      return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar criar pedido. Erro: {ex.Message}");
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Put(int id, OrderInputDto model)
  {
    {
      try
      {
        var order = await _orderService.UpdateOrder(id, model);
        if (order == null) return BadRequest("Não foi possível atualizar o pedido");

        return Ok(order);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar pedido. Erro: {ex.Message}");
      }
    }
  }

  [HttpPatch("{id}/{step}")]
  public async Task<IActionResult> UpdateStep(int id, int step)
  {
    {
      try
      {
        var order = await _orderService.UpdateStep(id, step);
        if (order == null) return BadRequest("Não foi possível atualizar o pedido");

        return Ok(order);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar pedido. Erro: {ex.Message}");
      }
    }
  }


  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      return await _orderService.DeleteOrder(id) ? Ok(new { message = "Deletado com sucesso" }) : throw new Exception("Houve um erro ao deletar o pedido");
    }
    catch (Exception ex)
    {
      return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao deletar pedido. Erro: {ex.Message}");
    }
  }
}

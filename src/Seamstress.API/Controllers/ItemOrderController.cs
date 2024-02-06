using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class ItemOrderController : ControllerBase
  {
    private readonly IItemOrderService _itemOrderService;

    public ItemOrderController(IItemOrderService itemOrderService)
    {
      this._itemOrderService = itemOrderService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      try
      {
        var itemOrder = await _itemOrderService.GetItemOrderById(id);
        if (itemOrder == null) return NoContent();

        return Ok(itemOrder);
      }
      catch (Exception ex)
      {

        return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar o item. Erro: {ex.Message}");
      }
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetByOrderId(int orderId)
    {
      try
      {
        var itemOrders = await _itemOrderService.GetItemOrdersByOrderId(orderId);
        if (itemOrders == null) return NoContent();

        return Ok(itemOrders);
      }
      catch (Exception ex)
      {

        return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar os itens. Erro: {ex.Message}");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post(ItemOrderInputDto model)
    {
      try
      {
        var itemOrder = await _itemOrderService.AddItemOrder(model);
        if (itemOrder == null) return BadRequest("Não foi possível adicionar o item ao pedido");

        return Ok(await _itemOrderService.GetItemOrderById(itemOrder.Id));
      }
      catch (Exception ex)
      {

        return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível criar o item de pedido. Erro: {ex.Message}");
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        return await _itemOrderService.DeleteItemOrder(id) ? Ok(new { message = "Deletado com sucesso" }) : BadRequest("Não foi possível deletar o item do pedido");
      }
      catch (Exception ex)
      {

        return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível deletar o item de pedido. Erro: {ex.Message}");
      }
    }

  }
}
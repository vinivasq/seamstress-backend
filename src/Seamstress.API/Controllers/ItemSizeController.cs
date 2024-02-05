using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Domain;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class ItemSizeController : ControllerBase
  {
    private readonly IItemSizeService _itemSizeService;

    public ItemSizeController(IItemSizeService itemSizeService)
    {
      this._itemSizeService = itemSizeService;
    }

    [HttpGet("item/{id}")]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var itemSizes = await _itemSizeService.GetItemSizesByItemIdAsync(id);
        if (itemSizes == null) return NoContent();

        return Ok(itemSizes);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar os tamanhos. Erro: {ex.Message}");
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      try
      {
        var itemSize = await _itemSizeService.GetItemSizeByIdAsync(id);
        if (itemSize == null) return NoContent();

        return Ok(itemSize);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar o tamanho. Erro: {ex.Message}");
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeasurements(int id, ItemSize model)
    {
      try
      {
        var itemSize = await _itemSizeService.UpdateItemSizeAsync(id, model);
        if (itemSize == null) return BadRequest("Não foi possível criar o tamanho");

        return Ok(itemSize);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível atualizar o tamanho. Erro: {ex.Message}");
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        return await _itemSizeService.DeleteItemSizeAsync(id) ? Ok(new { message = "Tamanho deletado com sucesso." }) : BadRequest("Não foi possível deletar o tamaho");
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível deletar o tamanho. Erro: {ex.Message}");
      }
    }

  }
}
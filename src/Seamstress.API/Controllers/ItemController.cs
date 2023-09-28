using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;

namespace Seamstress.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ItemController : ControllerBase
  {
    private readonly IItemService _itemService;
    private readonly IImageService _imageService;
    public ItemController(IItemService itemService, IImageService imageService)
    {
      this._itemService = itemService;
      this._imageService = imageService;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
      try
      {
        var items = await _itemService.GetItemsAsync();
        if (items == null) NoContent();

        return Ok(items);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar os modelos. Erro: {ex.Message}");
      }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
      try
      {
        var item = await _itemService.GetItemByIdAsync(id);
        if (item == null) return NoContent();

        return Ok(item);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar o modelo. Erro: {ex.Message}");
      }
    }

    [HttpPost]
    public async Task<ActionResult> Post(ItemInputDto model)
    {
      try
      {
        var item = await _itemService.AddItem(model);
        if (item == null) return BadRequest("Não foi possível criar o item");

        return Ok(item);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível criar o modelo. Erro: {ex.Message}");
      }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, ItemInputDto model)
    {
      try
      {
        var item = await _itemService.UpdateITem(id, model);
        if (item == null) return BadRequest();

        return Ok(item);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível atualizar o modelo. Erro: {ex.Message}");
      }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
      try
      {
        return await _itemService.DeleteITem(id) ? Ok(new { message = "Deletado com sucesso" }) : BadRequest();
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível excluir o modelo. Erro: {ex.Message}");
      }
    }

    [HttpPost("image/{itemId}")]
    public async Task<IActionResult> UploadImage(int itemId)
    {
      try
      {
        var files = Request.Form.Files.ToList();
        string filesResponse = await this._imageService.UpdateImage(files, itemId);

        return Ok(new { imageURL = filesResponse });
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível atualizar a imagem. Erro: {ex.Message}");
      }
    }


  }
}
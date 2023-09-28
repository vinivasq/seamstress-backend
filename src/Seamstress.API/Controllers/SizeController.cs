using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Domain;

namespace Seamstress.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class SizeController : ControllerBase
  {
    private readonly ISizeService _sizeService;
    public SizeController(ISizeService sizeService)
    {
      this._sizeService = sizeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      try
      {
        var size = await _sizeService.GetSizesAsync();
        if (size == null) return NoContent();

        return Ok(size);
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
        var size = await _sizeService.GetSizeByIdAsync(id);
        if (size == null) return NoContent();

        return Ok(size);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar o tamanho. Erro: {ex.Message}");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post(Size model)
    {
      try
      {
        var size = await _sizeService.AddSize(model);
        if (size == null) return BadRequest("Não foi possível criar o tamanho");

        return Ok(size);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível criar o tamanho. Erro: {ex.Message}");
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        return await _sizeService.DeleteSize(id) ? Ok(new { message = "Tamanho deletado com sucesso." }) : BadRequest("Não foi possível deletar o tamaho");
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível deletar o tamanho. Erro: {ex.Message}");
      }
    }

  }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Domain;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class ColorController : ControllerBase
  {
    private readonly IColorService _colorService;
    public ColorController(IColorService colorService)
    {
      this._colorService = colorService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      try
      {
        var colors = await _colorService.GetColorsAsync();
        if (colors == null) return NoContent();

        return Ok(colors);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar as cores. Erro: {ex.Message}");
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      try
      {
        var color = await _colorService.GetColorByIdAsync(id);
        if (color == null) return NoContent();

        return Ok(color);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar a cor. Erro: {ex.Message}");
      }
    }

    [HttpGet("fk/{id}")]
    public async Task<IActionResult> CheckFK(int id)
    {
      try
      {
        return Ok(await _colorService.CheckFK(id));
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar a cor. Erro: {ex.Message}");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post(Color model)
    {
      try
      {
        var color = await _colorService.AddColor(model);
        if (color == null) return BadRequest("Não foi possível criar a cor");

        return Ok(color);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível criar a cor. Erro: {ex.Message}");
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Color model)
    {
      try
      {
        var color = await _colorService.UpdateColor(id, model);
        if (color == null) return BadRequest("Não foi possível atualizar a cor");

        return Ok(color);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível editar a cor. Erro: {ex.Message}");
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        return await _colorService.DeleteColor(id) ? Ok(new { message = "Cor deletada com sucesso." }) : BadRequest("Não foi possível deletar a cor");
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível deletar a cor. Erro: {ex.Message}");
      }
    }

  }
}
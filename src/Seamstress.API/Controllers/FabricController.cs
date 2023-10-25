using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Domain;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class FabricController : ControllerBase
  {
    private readonly IFabricService _fabricService;
    public FabricController(IFabricService fabricService)
    {
      this._fabricService = fabricService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      try
      {
        var colors = await _fabricService.GetFabricsAsync();
        if (colors == null) return NoContent();

        return Ok(colors);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar os tecidos. Erro: {ex.Message}");
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      try
      {
        var color = await _fabricService.GetFabricByIdAsync(id);
        if (color == null) return NoContent();

        return Ok(color);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar o tecido. Erro: {ex.Message}");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post(Fabric model)
    {
      try
      {
        var fabric = await _fabricService.AddFabric(model);
        if (fabric == null) return BadRequest("Não foi possível criar o tecido");

        return Ok(fabric);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível criar o tecido. Erro: {ex.Message}");
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Fabric model)
    {
      try
      {
        var fabric = await _fabricService.UpdateFabric(id, model);
        if (fabric == null) return BadRequest("Não foi possível atualizar o tecido");

        return Ok(fabric);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível editar o tecido. Erro: {ex.Message}");
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        return await _fabricService.DeleteFabric(id) ? Ok(new { message = "Tecido deletada com sucesso." }) : BadRequest("Não foi possível deletar o tecido");
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível deletar o tecido. Erro: {ex.Message}");
      }
    }

  }
}
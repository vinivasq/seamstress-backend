using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Domain;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class SetController : ControllerBase
  {
    private readonly ISetService _setService;
    public SetController(ISetService setService)
    {
      this._setService = setService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      try
      {
        var sets = await _setService.GetSetsAsync();
        if (sets == null) return NoContent();

        return Ok(sets);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar os conjuntos. Erro: {ex.Message}");
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      try
      {
        var set = await _setService.GetSetByIdAsync(id);
        if (set == null) return NoContent();

        return Ok(set);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível listar o conjunto. Erro: {ex.Message}");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post(Set model)
    {
      try
      {
        var set = await _setService.AddSet(model);
        if (set == null) return BadRequest("Não foi possível criar o conjunto");

        return Ok(set);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível cadastrar o conjunto. Erro: {ex.Message}");
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Set model)
    {
      try
      {
        var set = await _setService.UpdateSet(id, model);
        if (set == null) return BadRequest("Não foi possível atualizar o conjunto");

        return Ok(set);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível atualizar o conjunto. Erro: {ex.Message}");
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        return await _setService.DeleteSet(id) ? Ok(new { message = "Deletado com sucesso." }) : BadRequest("Não foi possível deletar o conjunto.");
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível atualizar o conjunto. Erro: {ex.Message}");
      }
    }
  }
}
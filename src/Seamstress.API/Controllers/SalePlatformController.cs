using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.Application.Contracts;
using Seamstress.Domain;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class SalePlatformController : ControllerBase
  {
    private readonly ISalePlatformService _salePlatformService;

    public SalePlatformController(ISalePlatformService salePlatformService)
    {
      this._salePlatformService = salePlatformService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSalesPlatform()
    {
      try
      {
        IEnumerable<SalePlatform> salePlatforms = await this._salePlatformService.GetSalesPlatformAsync();
        if (salePlatforms is null || !salePlatforms.Any()) return NoContent();

        return Ok(salePlatforms);
      }
      catch (Exception ex)
      {
        return BadRequest($"Erro ao recuperar as plataformas de vendas. Erro: {ex.Message}");
      }
    }

  }
}
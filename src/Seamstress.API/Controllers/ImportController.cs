using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.API.Extensions;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain.Enum;
using System.Text.Json;

namespace Seamstress.API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class ImportController : ControllerBase
  {
    private readonly IImportService _importService;
    private readonly IUserService _userService;

    public ImportController(IImportService importService, IUserService userService)
    {
      _importService = importService;
      _userService = userService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] int salePlatformId)
    {
      try
      {
        var role = await GetUserRole();
        if (role != Roles.Admin.ToString() && role != Roles.Requester.ToString())
          return StatusCode(StatusCodes.Status403Forbidden, "Acesso negado.");

        if (file == null || file.Length == 0)
          return BadRequest("Arquivo CSV não fornecido.");

        var result = await _importService.ParseCsvAsync(file, salePlatformId);
        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message}");
      }
    }

    [HttpPost("preview")]
    public async Task<IActionResult> Preview([FromBody] PreviewRequest request)
    {
      try
      {
        var role = await GetUserRole();
        if (role != Roles.Admin.ToString() && role != Roles.Requester.ToString())
          return StatusCode(StatusCodes.Status403Forbidden, "Acesso negado.");

        var result = await _importService.GeneratePreviewAsync(
            request.SessionId, request.Mappings, request.SalePlatformId);
        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message}");
      }
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute([FromBody] ExecuteRequest request)
    {
      try
      {
        var role = await GetUserRole();
        if (role != Roles.Admin.ToString() && role != Roles.Requester.ToString())
          return StatusCode(StatusCodes.Status403Forbidden, "Acesso negado.");

        var result = await _importService.ExecuteImportAsync(request.SessionId);
        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message}");
      }
    }

    [HttpGet("mapping/{salePlatformId}")]
    public async Task<IActionResult> GetMapping(int salePlatformId)
    {
      try
      {
        var role = await GetUserRole();
        if (role != Roles.Admin.ToString() && role != Roles.Requester.ToString())
          return StatusCode(StatusCodes.Status403Forbidden, "Acesso negado.");

        var mapping = await _importService.GetMappingAsync(salePlatformId);
        if (mapping == null) return NoContent();

        var mappings = JsonSerializer.Deserialize<List<ImportColumnMappingDto>>(mapping.MappingsJson);
        return Ok(mappings);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message}");
      }
    }

    [HttpPut("mapping")]
    public async Task<IActionResult> SaveMapping([FromBody] SaveMappingRequest request)
    {
      try
      {
        var role = await GetUserRole();
        if (role != Roles.Admin.ToString())
          return StatusCode(StatusCodes.Status403Forbidden, "Apenas administradores podem configurar mapeamentos.");

        var result = await _importService.SaveMappingAsync(request.SalePlatformId, request.Mappings);
        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message}");
      }
    }

    private async Task<string> GetUserRole()
    {
      var user = await _userService.GetUserByUserNameAsync(User.GetUserName());
      return user?.Role ?? "";
    }
  }

  // Request DTOs for controller-specific request shapes
  public class PreviewRequest
  {
    public string SessionId { get; set; } = null!;
    public List<ImportColumnMappingDto> Mappings { get; set; } = new();
    public int SalePlatformId { get; set; }
  }

  public class ExecuteRequest
  {
    public string SessionId { get; set; } = null!;
  }

  public class SaveMappingRequest
  {
    public int SalePlatformId { get; set; }
    public List<ImportColumnMappingDto> Mappings { get; set; } = new();
  }
}

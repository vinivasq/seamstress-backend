using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seamstress.API.Extensions;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain.Enum;

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

        [HttpPost("preview")]
        public async Task<IActionResult> Preview([FromBody] PreviewRequestDto request)
        {
            try
            {
                var user = await _userService.GetUserByUserNameAsync(User.GetUserName());
                var role = user?.Role ?? "";
                if (role != Roles.Admin.ToString() && role != Roles.Requester.ToString())
                    return StatusCode(StatusCodes.Status403Forbidden, "Acesso negado.");

                var result = await _importService.GeneratePreviewAsync(
                    request.Products, request.SalePlatformId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message}");
            }
        }

        [HttpPost("execute")]
        public async Task<IActionResult> Execute([FromBody] ExecuteRequestDto request)
        {
            try
            {
                var user = await _userService.GetUserByUserNameAsync(User.GetUserName());
                var role = user?.Role ?? "";
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
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Seamstress.API.Extensions;
using Seamstress.Application.Contracts;
using Seamstress.Domain.Enum;

namespace Seamstress.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NuvemShopController : ControllerBase
    {
        private readonly INuvemShopService _nuvemShopService;
        private readonly IUserService _userService;

        public NuvemShopController(INuvemShopService nuvemShopService, IUserService userService)
        {
            _nuvemShopService = nuvemShopService;
            _userService = userService;
        }

        [HttpGet("import-preview")]
        public async Task<IActionResult> GetImportPreview()
        {
            try
            {
                var user = await _userService.GetUserByUserNameAsync(User.GetUserName());
                var role = user?.Role ?? "";
                if (role != Roles.Admin.ToString() && role != Roles.Requester.ToString())
                    return StatusCode(StatusCodes.Status403Forbidden, "Acesso negado.");

                var preview = await _nuvemShopService.FetchAndPreviewAsync();
                return Ok(preview);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

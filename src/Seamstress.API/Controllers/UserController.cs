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
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public UserController(IUserService userService,
                          ITokenService tokenService)
    {
      this._tokenService = tokenService;
      this._userService = userService;
    }

    [HttpGet("GetUser")]
    public async Task<IActionResult> GetUser()
    {
      try
      {
        var username = User.GetUserName();
        var user = await _userService.GetUserByUserNameAsync(username) ?? throw new Exception("Não foi possível encontrar o usuário");
        return Ok(user);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível recuperar o usuário. Erro: {ex.Message}");
      }
    }

    [HttpGet("executors")]
    public async Task<IActionResult> GetAllExecutors()
    {
      try
      {
        var users = await _userService.GetAllExecutorsAsync();
        return Ok(users);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível retornar os usuários. Erro: {ex.Message}");
      }
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserDto userDto)
    {
      try
      {
        if (await _userService.UserExists(userDto.UserName)) return BadRequest("Usuário já existe");

        var user = await _userService.CreateAccountAsync(userDto) ?? throw new Exception("Não foi possível criar a conta.");
        return Ok(new
        {
          Id = user.Id,
          UserName = user.UserName,
          FirstName = user.FirstName,
          Role = user.Role,
          Token = _tokenService.CreateToken(user).Result
        }
        );
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível registrar o usuário. Erro: {ex.Message}");
      }
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserLoginDto userLogin)
    {
      try
      {
        var user = await _userService.GetUserByUserNameAsync(userLogin.UserName);
        if (user == null) return Unauthorized("Usuário ou senha inválidos.");

        var result = await _userService.CheckUserPasswordAsync(user, userLogin.Password);
        if (!result.Succeeded) return Unauthorized("Usuário ou senha inválidos.");

        return Ok(new
        {
          Id = user.Id,
          UserName = user.UserName,
          FirstName = user.FirstName,
          Role = user.Role,
          Token = _tokenService.CreateToken(user).Result
        }
        );
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível autenticar o usuário. Erro: {ex.Message}");
      }
    }

    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser(UserUpdateDto userUpdateDto)
    {
      try
      {
        var user = await _userService.GetUserByUserNameAsync(User.GetUserName());
        if (user == null) return Unauthorized("Usuário inválido.");

        var userResponse = await _userService.UpdateAccount(userUpdateDto);
        if (userResponse == null) return BadRequest("Não foi possível atualizar o usuário");

        return Ok(userResponse);
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível atualizar o usuário. Erro: {ex.Message}");
      }
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
      try
      {
        var requestingUser = await _userService.GetUserByUserNameAsync(User.GetUserName());
        if (requestingUser.Role != Roles.Admin.ToString()) return StatusCode(StatusCodes.Status403Forbidden, "Acesso negado.");

        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível retornar os usuários. Erro: {ex.Message}");
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AdminUpdateUser(int id, AdminUserUpdateDto dto)
    {
      try
      {
        var requestingUser = await _userService.GetUserByUserNameAsync(User.GetUserName());
        if (requestingUser.Role != Roles.Admin.ToString()) return StatusCode(StatusCodes.Status403Forbidden, "Acesso negado.");

        var updatedUser = await _userService.AdminUpdateUserAsync(id, dto);
        return Ok(updatedUser);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível atualizar o usuário. Erro: {ex.Message}");
      }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
      try
      {
        await _userService.ChangePasswordAsync(User.GetUserName(), dto);
        return Ok();
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível alterar a senha. Erro: {ex.Message}");
      }
    }

    [AllowAnonymous]
    [HttpGet("Validate/{token}")]
    public IActionResult ValidateToken(string token)
    {
      try
      {
        return Ok(this._tokenService.ValidateToken(token));
      }
      catch (Exception ex)
      {

        return this.StatusCode(StatusCodes.Status401Unauthorized, $"Não foi possível autenticar o usuário. Erro: {ex.Message}");
      }
    }

  }
}
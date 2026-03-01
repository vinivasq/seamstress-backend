using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain.Identity;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class UserService : IUserService
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;
    private readonly IUserPersistence _userPersistence;
    private readonly IGeneralPersistence _generalPersistence;

    public UserService(UserManager<User> userManager,
                        SignInManager<User> signInManager,
                        IMapper mapper,
                        IUserPersistence userPersistence,
                        IGeneralPersistence generalPersistence
                      )
    {
      this._userManager = userManager;
      this._signInManager = signInManager;
      this._mapper = mapper;
      this._userPersistence = userPersistence;
      this._generalPersistence = generalPersistence;
    }

    public async Task<UserOutputDto[]> GetAllUsersAsync()
    {
      try
      {
        var users = await _userPersistence.GetAllUsersAsync() ?? throw new Exception("Nenhum usuário encontrado");

        List<UserOutputDto> lstUsersDto = _mapper.Map<UserOutputDto[]>(users).ToList();

        lstUsersDto.ForEach(userDto =>
        {
          userDto.Name = users.Where(user => user.Id == userDto.Id).Select(user => $"{user.FirstName} {user.LastName}").First();
        });

        return lstUsersDto.ToArray();
      }
      catch (Exception ex)
      {
        throw new Exception($"Erro ao recuperar os usuários. Erro {ex.Message}");
      }
    }

    public async Task<UserOutputDto> AdminUpdateUserAsync(int id, AdminUserUpdateDto dto)
    {
      try
      {
        var user = await _userManager.FindByIdAsync(id.ToString())
          ?? throw new Exception($"Não foi encontrado um usuário de Id: {id}");

        if (user.UserName != dto.UserName.ToLower())
        {
          if (await _userManager.Users.AnyAsync(u => u.UserName == dto.UserName.ToLower()))
            throw new Exception("Nome de usuário já está em uso");

          user.UserName = dto.UserName.ToLower();
          user.NormalizedUserName = dto.UserName.ToUpper();
        }

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
          var token = await _userManager.GeneratePasswordResetTokenAsync(user);
          var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);
          if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        if (dto.Role.HasValue)
          user.Role = dto.Role.Value;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
          throw new Exception(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        var updatedUser = await _userPersistence.GetUserByIdAsync(id);
        var userDto = _mapper.Map<UserOutputDto>(updatedUser);
        userDto.Name = $"{updatedUser.FirstName} {updatedUser.LastName}";
        return userDto;
      }
      catch (Exception ex)
      {
        throw new Exception($"Erro ao atualizar o usuário. Erro: {ex.Message}");
      }
    }

    public async Task ChangePasswordAsync(string username, ChangePasswordDto dto)
    {
      try
      {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username.ToLower())
          ?? throw new Exception("Usuário não encontrado");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
          throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
      }
      catch (Exception ex)
      {
        throw new Exception($"Erro ao alterar a senha. Erro: {ex.Message}");
      }
    }

    public async Task<UserOutputDto[]> GetAllExecutorsAsync()
    {
      try
      {
        var users = await _userPersistence.GetAllExecutorsAsync() ?? throw new Exception("Nenhum usuário encontrado");

        List<UserOutputDto> lstUsersDto = _mapper.Map<UserOutputDto[]>(users).ToList();

        lstUsersDto.ForEach(userDto =>
        {
          userDto.Name = users.Where(user => user.Id == userDto.Id).Select(user => $"{user.FirstName} {user.LastName}").First();
        });

        return lstUsersDto.ToArray();
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao recuperar os usuáios. Erro {ex.Message}");
      }
    }


    public async Task<UserUpdateDto> GetUserByUserNameAsync(string username)
    {
      try
      {
        var user = await _userPersistence.GetUserByUserNameAsync(username)
        ?? throw new Exception("Não foi possível localizar o usuário");
        return _mapper.Map<UserUpdateDto>(user);
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao recuperar o usuário pelo nome de usuário. Erro: {ex.Message}");
      }
    }

    public async Task<SignInResult> CheckUserPasswordAsync(UserUpdateDto userUpdateDto, string password)
    {
      try
      {
        var user = await _userManager.Users.FirstOrDefaultAsync(user => user.UserName == userUpdateDto.UserName.ToLower())
        ?? throw new Exception("Não foi possível encontrar o usuário");
        return await _signInManager.CheckPasswordSignInAsync(user, password, false);
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao verificar a senha. Erro: {ex.Message}");
      }
    }

    public async Task<UserUpdateDto> CreateAccountAsync(UserDto userDto)
    {
      try
      {
        var user = _mapper.Map<User>(userDto);
        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (result.Succeeded) return _mapper.Map<UserUpdateDto>(user);

        throw new Exception("Não houve sucesso na criação de conta");
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao criar a conta. Erro: {ex.Message}");
      }
    }

    public async Task<UserUpdateDto> UpdateAccount(UserUpdateDto userUpdateDto)
    {
      try
      {
        var user = await _userPersistence.GetUserByUserNameAsync(userUpdateDto.UserName)
        ?? throw new Exception("Não foi possível localizar o usuário a atualizar");

        _mapper.Map(userUpdateDto, user);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, userUpdateDto.Password);

        _generalPersistence.Update<User>(user);

        if (await _generalPersistence.SaveChangesAsync())
          return _mapper.Map<UserUpdateDto>(await _userPersistence.GetUserByUserNameAsync(user.UserName));

        throw new Exception("Não foi possível atualizar o usuário");
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao atualizar a conta. Erro: {ex.Message}");
      }
    }

    public async Task<bool> UserExists(string username)
    {
      try
      {
        return await _userManager.Users.AnyAsync(user => user.UserName == username.ToLower());
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao verificar usuário. Erro: {ex.Message}");
      }
    }
  }
}
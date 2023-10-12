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

    public async Task<UserDto> CreateAccountAsync(UserDto userDto)
    {
      try
      {
        var user = _mapper.Map<User>(userDto);
        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (result.Succeeded) return _mapper.Map<UserDto>(user);

        throw new Exception("Não houve sucesso na criação de conta");
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao criar a conta. Erro: {ex.Message}");
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
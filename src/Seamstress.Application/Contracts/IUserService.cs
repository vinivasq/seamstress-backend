using Microsoft.AspNetCore.Identity;
using Seamstress.Application.Dtos;

namespace Seamstress.Application.Contracts
{
  public interface IUserService
  {
    Task<bool> UserExists(string username);
    Task<UserUpdateDto> GetUserByUsernameAsync(string username);
    Task<SignInResult> CheckUserPasswordAsync(UserUpdateDto userUpdateDto, string password);
    Task<UserDto> CreateAccountAsync(UserDto userDto);
    Task<UserUpdateDto> UpdateAccount(UserUpdateDto userUpdateDto);
  }
}
using Microsoft.AspNetCore.Identity;
using Seamstress.DTO;

namespace Seamstress.Application.Contracts
{
  public interface IUserService
  {
    Task<bool> UserExists(string username);
    Task<UserOutputDto[]> GetAllExecutorsAsync();
    Task<UserUpdateDto> GetUserByUserNameAsync(string username);
    Task<SignInResult> CheckUserPasswordAsync(UserUpdateDto userUpdateDto, string password);
    Task<UserUpdateDto> CreateAccountAsync(UserDto userDto);
    Task<UserUpdateDto> UpdateAccount(UserUpdateDto userUpdateDto);
  }
}
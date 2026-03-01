using Microsoft.AspNetCore.Identity;
using Seamstress.Application.Dtos;

namespace Seamstress.Application.Contracts
{
  public interface IUserService
  {
    Task<bool> UserExists(string username);
    Task<UserOutputDto[]> GetAllUsersAsync();
    Task<UserOutputDto[]> GetAllExecutorsAsync();
    Task<UserUpdateDto> GetUserByUserNameAsync(string username);
    Task<SignInResult> CheckUserPasswordAsync(UserUpdateDto userUpdateDto, string password);
    Task<UserUpdateDto> CreateAccountAsync(UserDto userDto);
    Task<UserUpdateDto> UpdateAccount(UserUpdateDto userUpdateDto);
    Task<UserOutputDto> AdminUpdateUserAsync(int id, AdminUserUpdateDto dto);
    Task ChangePasswordAsync(string username, ChangePasswordDto dto);
  }
}
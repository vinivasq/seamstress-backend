using Seamstress.Application.Dtos;

namespace Seamstress.Application.Contracts
{
  public interface ITokenService
  {
    Task<string> CreateToken(UserUpdateDto userUpdateDto);
    bool ValidateToken(string token);
  }
}
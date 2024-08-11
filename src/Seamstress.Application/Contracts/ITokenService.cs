using Seamstress.DTO;

namespace Seamstress.Application.Contracts
{
  public interface ITokenService
  {
    Task<string> CreateToken(UserUpdateDto userUpdateDto);
    bool ValidateToken(string token);
  }
}
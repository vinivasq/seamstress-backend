using Seamstress.Domain.Enum;

namespace Seamstress.DTO
{
  public class UserDto
  {
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Roles Role { get; set; }
  }
}
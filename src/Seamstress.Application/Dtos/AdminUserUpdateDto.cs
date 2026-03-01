using Seamstress.Domain.Enum;

namespace Seamstress.Application.Dtos
{
  public class AdminUserUpdateDto
  {
    public string UserName { get; set; } = null!;
    public string? Password { get; set; }
    public Roles? Role { get; set; }
  }
}

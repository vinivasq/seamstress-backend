using Seamstress.Domain.Enum;

namespace Seamstress.Application.Dtos
{
  public class UserDto
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Userame { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public Role Role { get; set; }
  }
}
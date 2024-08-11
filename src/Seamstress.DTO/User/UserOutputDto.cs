using Seamstress.Domain.Enum;

namespace Seamstress.DTO
{
  public class UserOutputDto
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Roles Role { get; set; }
  }
}
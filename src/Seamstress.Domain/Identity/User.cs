using Microsoft.AspNetCore.Identity;
using Seamstress.Domain.Enum;

namespace Seamstress.Domain.Identity
{
  public class User : IdentityUser
  {
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? ImageURL { get; set; }
  }
}
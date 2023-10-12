using Microsoft.AspNetCore.Identity;
using Seamstress.Domain.Enum;

namespace Seamstress.Domain.Identity
{
  public class User : IdentityUser<int>
  {
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Roles Role { get; set; }
    public IEnumerable<UserRole> UserRoles { get; set; } = null!;
    public string? ImageURL { get; set; }
  }
}
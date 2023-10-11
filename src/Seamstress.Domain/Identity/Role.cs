using Microsoft.AspNetCore.Identity;

namespace Seamstress.Domain.Identity
{
  public class Role : IdentityRole<int>
  {
    public IEnumerable<UserRole> UserRoles { get; set; } = null!;
  }
}
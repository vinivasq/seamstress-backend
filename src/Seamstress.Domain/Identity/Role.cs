using Microsoft.AspNetCore.Identity;

namespace Seamstress.Domain.Identity
{
  public class Role : IdentityRole
  {
    public IEnumerable<User> Users { get; set; } = null!;
  }
}
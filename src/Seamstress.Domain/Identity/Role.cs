using Microsoft.AspNetCore.Identity;
using Seamstress.Domain.Enum;

namespace Seamstress.Domain.Identity
{
  public class Role : IdentityRole<int>
  {
    public IEnumerable<UserRole> UserRoles { get; set; } = null!;
    public Roles Admin { get; set; } = Roles.Admin;
    public Roles Requester { get; set; } = Roles.Requester;
    public Roles Executor { get; set; } = Roles.Executor;
  }
}
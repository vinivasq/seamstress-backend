using Seamstress.Domain.Enum;

namespace Seamstress.Domain
{
  public class User
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Userame { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public IEnumerable<OrderUser> OrdersUser { get; set; } = null!;
    public Role Role { get; set; }
  }
}
using Seamstress.Domain.Identity;

namespace Seamstress.Domain
{
  public class OrderUser
  {
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
  }
}
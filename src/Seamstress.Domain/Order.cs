using Seamstress.Domain.Enum;
using Seamstress.Domain.Identity;

namespace Seamstress.Domain
{
  public class Order
  {
    public int Id { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Deadline { get; set; }
    public IEnumerable<ItemOrder> ItemOrders { get; set; } = null!;
    public int ExecutorId { get; set; }
    public decimal Total { get; set; }
    public Step Step { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; } = null!;
  }
}
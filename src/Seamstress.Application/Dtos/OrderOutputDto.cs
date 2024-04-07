using Seamstress.Domain;
using Seamstress.Domain.Enum;

namespace Seamstress.Application.Dtos
{
  public class OrderOutputDto
  {
    public int Id { get; set; }
    public string? Description { get; set; }
    public UserOutputDto Executor { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime OrderedAt { get; set; }
    public DateTime Deadline { get; set; }
    public decimal Total { get; set; }
    public Step Step { get; set; }
    public SalePlatform SalePlatform { get; set; } = null!;
    public CustomerOutputDto Customer { get; set; } = null!;
    public IEnumerable<ItemOrderOutputDto> ItemOrders { get; set; } = null!;
  }
}
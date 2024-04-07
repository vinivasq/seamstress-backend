using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IChartPersistence
  {
    public Task<List<Customer>> GetRegionCustomersAsync(DateOnly periodBegin, DateOnly periodEnd);
    public Task<List<ItemOrder>> GetModelItemOrdersAsync(DateOnly periodBegin, DateOnly periodEnd);
    public Task<List<Order>> GetOrdersAsync(DateOnly periodBegin, DateOnly periodEnd);
  }
}
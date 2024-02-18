using Seamstress.Application.Dtos;
using Seamstress.Persistence.Helpers;

namespace Seamstress.Application.Contracts
{
  public interface IOrderService
  {
    Task<OrderOutputDto> AddOrder(OrderInputDto model);
    Task<OrderOutputDto> UpdateStep(int id, int step);
    Task<OrderOutputDto> UpdateOrder(int id, OrderInputDto model);
    Task<bool> DeleteOrder(int id);

    Task<PageList<OrderOutputDto>> GetOrdersAsync(OrderParams orderParams);
    Task<OrderOutputDto[]> GetPendingOrdersAsync();
    Task<OrderOutputDto[]> GetPendingOrdersByExecutorAsync(int userId);
    Task<OrderOutputDto> GetOrderByIdAsync(int id);
  }
}
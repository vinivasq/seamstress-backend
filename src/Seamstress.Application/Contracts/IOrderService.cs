using Seamstress.Application.Dtos;

namespace Seamstress.Application.Contracts
{
  public interface IOrderService
  {
    Task<OrderOutputDto> AddOrder(OrderInputDto model);
    Task<OrderOutputDto> UpdateStep(int id, int step);
    Task<OrderOutputDto> UpdateOrder(int id, OrderInputDto model);
    Task<bool> DeleteOrder(int id);

    Task<OrderOutputDto[]> GetAllOrdersAsync();
    Task<OrderOutputDto> GetOrderByIdAsync(int id);
  }
}
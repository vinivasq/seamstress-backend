using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Persistence
{
  public class ChartPersistence : IChartPersistence
  {
    private readonly SeamstressContext _context;

    public ChartPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public async Task<DoughnutChart> GetModelDoughnutChartAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        List<ItemOrder> itemOrders = await _context.ItemOrder.Where(x =>
          DateOnly.FromDateTime(x.Order!.OrderedAt) >= periodBegin &&
          DateOnly.FromDateTime(x.Order.OrderedAt) <= periodEnd
        )
        .Include(x => x.Item)
        .AsNoTracking().ToListAsync();

        DoughnutChart doughnutChart = new() { };

        foreach (var item in itemOrders.GroupBy(x => x.Item!.Id)
                                        .Select(group => new
                                        {
                                          group.First().Item!.Name,
                                          Count = group.Count()
                                        }).OrderByDescending(x => x.Count))
        {
          doughnutChart.DataSets.Add(item.Count);
          doughnutChart.Labels.Add(item.Name);
        }

        return doughnutChart;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<DoughnutChart> GetRegionDoughnutChartAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        List<Customer> customers = await _context.Orders.Where(x =>
          DateOnly.FromDateTime(x.OrderedAt) >= periodBegin &&
          DateOnly.FromDateTime(x.OrderedAt) <= periodEnd
        )
        .Include(x => x.Customer)
        .Select(x => x.Customer!)
        .AsNoTracking().ToListAsync();

        DoughnutChart doughnutChart = new() { };

        foreach (var customer in customers.GroupBy(x => x.UF)
                                          .Select(group => new
                                          {
                                            group.First().UF,
                                            Count = group.Count()
                                          }).OrderByDescending(x => x.Count))
        {
          doughnutChart.DataSets.Add(customer.Count);
          doughnutChart.Labels.Add(customer.UF);
        }

        return doughnutChart;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
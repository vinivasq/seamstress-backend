using System.Globalization;
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

    public async Task<BarLineChart> GetOrdersBarLineChartAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        BarLineChart barLineChart = new() { };
        List<Order> orders = await this._context.Orders.Where(x =>
          DateOnly.FromDateTime(x.OrderedAt) >= periodBegin &&
          DateOnly.FromDateTime(x.OrderedAt) <= periodEnd
        )
        .Include(x => x.SalePlatform)
        .OrderBy(x => x.OrderedAt)
        .AsNoTracking().ToListAsync();

        if (orders.Count == 0) return barLineChart;

        List<SalePlatform> salePlatforms = orders.Select(x => x.SalePlatform!).Where(x => x != null).DistinctBy(x => x.Id).ToList();

        salePlatforms.ForEach(platform =>
        {
          barLineChart.DataSets.Add(new()
          {
            Data = new() { },
            Type = "bar",
            SalePlatform = platform
          }
          );
        });

        int daysBetween = (periodEnd.ToDateTime(new()) - periodBegin.ToDateTime(new())).Days;

        if (daysBetween <= 7)
        {

          var ordersByDayOfWeek = orders
            .GroupBy(order => order.OrderedAt)
            .SelectMany(group =>
              group.GroupBy(order => order.SalePlatformId)
                .Select(platformGroup => new
                {
                  group.Key,
                  DayOfWeek = group.Key.ToString("dddd", new CultureInfo("pt-BR")),
                  Platform = platformGroup.First().SalePlatform!,
                  Count = platformGroup.Count(),
                }
                )
              )
            .OrderBy(x => x.Key);

          var daysOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;

          barLineChart.Labels.AddRange(daysOfWeek);

          for (int i = 0; i < daysOfWeek.Length; i++)
          {

            barLineChart.DataSets.ForEach(x => x.Data.Add(0));

            barLineChart.DataSets.ForEach(x =>
              x.Data[i] = ordersByDayOfWeek
                .Where(y => y.DayOfWeek == daysOfWeek[i] && y.Platform.Id == x.SalePlatform.Id).Select(x => x.Count).FirstOrDefault()
            );

          }

        }
        else if (daysBetween <= 31)
        {

          var ordersByWeek = orders.GroupBy(order =>
            CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                order.OrderedAt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday)
            )
            .SelectMany(group =>
              group.GroupBy(order => order.SalePlatformId)
                .Select(platformGroup => new
                {
                  Week = group.Key,
                  Platform = platformGroup.First().SalePlatform!,
                  Count = platformGroup.Count(),
                  Key = group.First().OrderedAt
                })
            )
            .OrderBy(x => x.Key);

          foreach (var order in ordersByWeek)
          {
            barLineChart.DataSets.First(x => x.SalePlatform.Id == order.Platform.Id)
              .Data.Add(order.Count);

            if (barLineChart.Labels.Any(x => x == order.Key.ToString("dd/MM")) == false)
            {
              barLineChart.Labels.Add(order.Key.ToString("dd/MM"));
            }
          }

        }
        else
        {

          var ordersByMonth = orders.GroupBy(order =>
            CultureInfo.CurrentCulture.Calendar.GetMonth(order.OrderedAt)
            )
            .SelectMany(group =>
              group.GroupBy(order => order.SalePlatformId)
                .Select(platformGroup => new
                {
                  Month = group.Key,
                  Platform = platformGroup.First().SalePlatform!,
                  Count = platformGroup.Count(),
                  Key = group.First().OrderedAt
                })
            )
            .OrderBy(x => x.Key);

          foreach (var order in ordersByMonth)
          {
            barLineChart.DataSets.First(x => x.SalePlatform.Id == order.Platform.Id)
              .Data.Add(order.Count);

            if (barLineChart.Labels.Any(x => x == order.Key.ToString("MMMM")) == false)
            {
              barLineChart.Labels.Add(order.Key.ToString("MMMM"));
            }
          }

        }

        barLineChart.DataSets.Add(new()
        {
          Data = new() { },
          Type = "line",
          SalePlatform = new()
          {
            Id = 0,
            Name = "Total"
          }
        });

        for (int i = 0; i < barLineChart.DataSets[0].Data.Count; i++)
        {
          int total = 0;
          int platfomsAmount = barLineChart.DataSets.Where(x => x.SalePlatform.Id != 0).Count();

          for (int y = 0; y < platfomsAmount; y++)
          {
            total += barLineChart.DataSets[y].Data[i];
          }

          barLineChart.DataSets.First(x => x.SalePlatform.Id == 0).Data.Add(total);
        }

        return barLineChart;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<BarLineChart> GetRevenueBarLineChartAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        List<Order> orders = await this._context.Orders.Where(x =>
          DateOnly.FromDateTime(x.OrderedAt) >= periodBegin &&
          DateOnly.FromDateTime(x.OrderedAt) <= periodEnd
        )
        .Include(x => x.SalePlatform)
        .AsNoTracking().ToListAsync();


        throw new NotImplementedException();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
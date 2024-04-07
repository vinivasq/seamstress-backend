using System.Globalization;
using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Application
{
  public class ChartService : IChartService
  {
    private readonly IChartPersistence _chartPersistence;
    private readonly IMapper _mapper;
    public CultureInfo PTBRCulture { get; set; }

    public ChartService(IChartPersistence chartPersistence, IMapper mapper)
    {
      this._chartPersistence = chartPersistence;
      this._mapper = mapper;
      this.PTBRCulture = new("pt-BR");
    }
    public async Task<DoughnutChart> GetRegionDoughnutChartAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        List<Customer> customers = await this._chartPersistence.GetRegionCustomersAsync(periodBegin, periodEnd);
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

    public async Task<DoughnutChart> GetModelDoughnutChartAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        List<ItemOrder> itemOrders = await this._chartPersistence.GetModelItemOrdersAsync(periodBegin, periodEnd);

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

    public async Task<BarLineChartDto> GetOrderBarLineChartAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        BarLineChart barLineChart = new() { };
        List<Order> orders = await this._chartPersistence.GetOrdersAsync(periodBegin, periodEnd);
        if (orders.Count == 0) return _mapper.Map<BarLineChartDto>(barLineChart);

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

          var daysOfWeek = PTBRCulture.DateTimeFormat.DayNames;

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
            PTBRCulture.Calendar.GetWeekOfYear(
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

          List<int> weeksOfMonth = ordersByWeek.Select(x => x.Week).Distinct().ToList();

          for (int i = 0; i < weeksOfMonth.Count; i++)
          {
            barLineChart.DataSets.ForEach(x => x.Data.Add(0));

            barLineChart.DataSets.ForEach(x =>
              x.Data[i] = ordersByWeek
                .Where(y => y.Week == weeksOfMonth[i] && y.Platform.Id == x.SalePlatform.Id).Select(x => x.Count).FirstOrDefault()
            );
          }

          barLineChart.Labels.AddRange(ordersByWeek.Select(x => $"Semana {x.Key:dd/MM}").Distinct());
        }
        else
        {

          var ordersByMonth = orders.GroupBy(order =>
            PTBRCulture.Calendar.GetMonth(order.OrderedAt)
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

          List<int> monthsOfYear = ordersByMonth.Select(x => x.Month).Distinct().ToList();

          for (int i = 0; i < monthsOfYear.Count; i++)
          {
            barLineChart.DataSets.ForEach(x => x.Data.Add(0));

            barLineChart.DataSets.ForEach(x =>
              x.Data[i] = ordersByMonth
                .Where(y => y.Month == monthsOfYear[i] && y.Platform.Id == x.SalePlatform.Id).Select(x => x.Count).FirstOrDefault()
            );
          }

          barLineChart.Labels.AddRange(ordersByMonth.Select(x => x.Key.ToString("MMMM")).Distinct());
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

        if (barLineChart.DataSets.Any(x => x.SalePlatform.Id == 99))
          barLineChart.DataSets.Remove(barLineChart.DataSets.Where(x => x.SalePlatform.Id == 99).First());

        return this._mapper.Map<BarLineChartDto>(barLineChart);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<RevenueBarLineChartDto> GetRevenueBarLineChartAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        RevenueBarLineChart revenueBarLineChart = new() { };
        List<Order> orders = await this._chartPersistence.GetOrdersAsync(periodBegin, periodEnd);
        if (orders.Count == 0) return _mapper.Map<RevenueBarLineChartDto>(revenueBarLineChart);

        List<SalePlatform> salePlatforms = orders.Select(x => x.SalePlatform!).Where(x => x != null).DistinctBy(x => x.Id).ToList();

        salePlatforms.ForEach(platform =>
        {
          revenueBarLineChart.DataSets.Add(new()
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
                  Revenue = platformGroup.Sum(x => x.Total),
                }
                )
              )
            .OrderBy(x => x.Key);

          var daysOfWeek = PTBRCulture.DateTimeFormat.DayNames;

          revenueBarLineChart.Labels.AddRange(daysOfWeek);

          for (int i = 0; i < daysOfWeek.Length; i++)
          {

            revenueBarLineChart.DataSets.ForEach(x => x.Data.Add(0));

            revenueBarLineChart.DataSets.ForEach(x =>
              x.Data[i] = ordersByDayOfWeek
                .Where(y => y.DayOfWeek == daysOfWeek[i] && y.Platform.Id == x.SalePlatform.Id).Select(x => x.Revenue).FirstOrDefault()
            );

          }

        }
        else if (daysBetween <= 31)
        {

          var ordersByWeek = orders.GroupBy(order =>
            PTBRCulture.Calendar.GetWeekOfYear(
                order.OrderedAt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday)
            )
            .SelectMany(group =>
              group.GroupBy(order => order.SalePlatformId)
                .Select(platformGroup => new
                {
                  Week = group.Key,
                  Platform = platformGroup.First().SalePlatform!,
                  Revenue = platformGroup.Sum(x => x.Total),
                  Key = group.First().OrderedAt
                })
            )
            .OrderBy(x => x.Key);

          List<int> weeksOfMonth = ordersByWeek.Select(x => x.Week).Distinct().ToList();

          for (int i = 0; i < weeksOfMonth.Count; i++)
          {
            revenueBarLineChart.DataSets.ForEach(x => x.Data.Add(0));

            revenueBarLineChart.DataSets.ForEach(x =>
              x.Data[i] = ordersByWeek
                .Where(y => y.Week == weeksOfMonth[i] && y.Platform.Id == x.SalePlatform.Id).Select(x => x.Revenue).FirstOrDefault()
            );
          }

          revenueBarLineChart.Labels.AddRange(ordersByWeek.Select(x => $"Semana {x.Key:dd/MM}").Distinct());

        }
        else
        {

          var ordersByMonth = orders.GroupBy(order =>
            PTBRCulture.Calendar.GetMonth(order.OrderedAt)
            )
            .SelectMany(group =>
              group.GroupBy(order => order.SalePlatformId)
                .Select(platformGroup => new
                {
                  Month = group.Key,
                  Platform = platformGroup.First().SalePlatform!,
                  Revenue = platformGroup.Sum(x => x.Total),
                  Key = group.First().OrderedAt
                })
            )
            .OrderBy(x => x.Key);

          List<int> monthsOfYear = ordersByMonth.Select(x => x.Month).Distinct().ToList();

          for (int i = 0; i < monthsOfYear.Count; i++)
          {
            revenueBarLineChart.DataSets.ForEach(x => x.Data.Add(0));

            revenueBarLineChart.DataSets.ForEach(x =>
              x.Data[i] = ordersByMonth
                .Where(y => y.Month == monthsOfYear[i] && y.Platform.Id == x.SalePlatform.Id).Select(x => x.Revenue).FirstOrDefault()
            );
          }

          revenueBarLineChart.Labels.AddRange(ordersByMonth.Select(x => x.Key.ToString("MMMM")).Distinct());
        }

        revenueBarLineChart.DataSets.Add(new()
        {
          Data = new() { },
          Type = "line",
          SalePlatform = new()
          {
            Id = 0,
            Name = "Total"
          }
        });

        for (int i = 0; i < revenueBarLineChart.DataSets[0].Data.Count; i++)
        {
          decimal total = 0;
          int platfomsAmount = revenueBarLineChart.DataSets.Where(x => x.SalePlatform.Id != 0).Count();

          for (int y = 0; y < platfomsAmount; y++)
          {
            total += revenueBarLineChart.DataSets[y].Data[i];
          }

          revenueBarLineChart.DataSets.First(x => x.SalePlatform.Id == 0).Data.Add(total);
        }

        if (revenueBarLineChart.DataSets.Any(x => x.SalePlatform.Id == 99))
          revenueBarLineChart.DataSets.Remove(revenueBarLineChart.DataSets.Where(x => x.SalePlatform.Id == 99).First());

        return this._mapper.Map<RevenueBarLineChartDto>(revenueBarLineChart);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
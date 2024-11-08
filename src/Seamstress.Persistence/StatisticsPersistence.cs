using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Persistence
{
  public class StatisticsPersistence : IStatisticsPersistence
  {
    private readonly SeamstressContext _context;

    public StatisticsPersistence(SeamstressContext context)
    {
      this._context = context;
    }
    public async Task<Statistic[]> GetStatisticsAsync()
    {
      CultureInfo cultureInfo = new("pt-BR");
      decimal currentRevenue = await this._context.Orders.Where(x =>
        DateOnly.FromDateTime(x.OrderedAt) >= DateOnly.FromDateTime(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1))
      ).SumAsync(x => x.Total);

      List<Statistic> statistics = new()
      {
        // new () Eliza pediu para remover visto que não refletia o faturamento real do atelie, já que muitas peças eram vendidas com valores diferentes do cadastrados no sistema
        // {
        //   Label = "Faturamento do mês atual",
        //   Value = currentRevenue.ToString("C2", cultureInfo)
        // },
        new ()
        {
          Label = "Melhor plataforma de venda",
          Value = Convert.ToString(
            await _context.Orders
              .Where(o => o.SalePlatformId != null && o.SalePlatformId != 99)
              .Include(x => x.SalePlatform)
              .GroupBy(o => o.SalePlatformId)
              .Select(group => new
              {
                Platform = group.First().SalePlatform!.Name,
                Count = group.Count()
              })
              .OrderByDescending(x => x.Count)
              .Select(x => x.Platform)
              .AsNoTracking().FirstOrDefaultAsync() ?? "Não informado"
          )
        },
        new ()
        {
          Label = "Região mais recorrente",
          Value = Convert.ToString(
            await this._context.Orders
            .Include(x => x.Customer)
            .GroupBy(x => x.Customer!.UF)
            .Select(group => new
            {
              UF = group.Key,
              Count = group.Count()
            }
            ).OrderByDescending(x => x.Count)
            .Select(x => x.UF)
            .AsNoTracking().FirstAsync()
          )
        },
        new ()
        {
          Label = "Modelo mais vendido",
          Value = Convert.ToString(
            await this._context.ItemOrder
              .Include(x => x.Item)
              .GroupBy(x => x.ItemId)
              .Select(group => new
              {
                group.First().Item!.Name,
                Count = group.Count()
              }
              ).OrderByDescending(x => x.Count)
              .Select(x => x.Name)
              .AsNoTracking().FirstAsync()
          )
        },
        new ()
        {
          Label = "Total de clientes",
          Value = Convert.ToString(await this._context.Customers.CountAsync())
        },
        new ()
        {
          Label = "Total de pedidos",
          Value = Convert.ToString(await this._context.Orders.CountAsync())
        },
        new ()
        {
          Label = "Total de modelos",
          Value = Convert.ToString(await this._context.Items.CountAsync())
        }
      };

      return statistics.ToArray();
    }
  }
}
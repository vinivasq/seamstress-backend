using Seamstress.Application.Contracts;
using Seamstress.Persistence.Contracts;
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Application
{
  public class ChartService : IChartService
  {
    private readonly IChartPersistence _chartPersistence;

    public ChartService(IChartPersistence chartPersistence)
    {
      this._chartPersistence = chartPersistence;
    }

    public async Task<PieChart> GetPieChartAsync(string data, DateTime periodBegin, DateTime periodEnd)
    {
      try
      {
        if (data.Trim().ToLower() == "region")
          return await this._chartPersistence.GetRegionPieChartAsync(periodBegin, periodEnd);
        if (data.Trim().ToLower() == "model")
          return await this._chartPersistence.GetModelPieChartAsync(periodBegin, periodEnd);

        throw new Exception("Tipo de dado inválido");
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
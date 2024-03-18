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

    public async Task<DoughnutChart> GetDoughnutChartAsync(string data, DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        if (data.Trim().ToLower() == "region")
          return await this._chartPersistence.GetRegionDoughnutChartAsync(periodBegin, periodEnd);
        if (data.Trim().ToLower() == "model")
          return await this._chartPersistence.GetModelDoughnutChartAsync(periodBegin, periodEnd);

        throw new Exception("Tipo de dado inválido");
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
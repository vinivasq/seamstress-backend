using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Persistence.Contracts;
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Application
{
  public class ChartService : IChartService
  {
    private readonly IChartPersistence _chartPersistence;
    private readonly IMapper _mapper;

    public ChartService(IChartPersistence chartPersistence, IMapper mapper)
    {
      this._chartPersistence = chartPersistence;
      this._mapper = mapper;
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

    public async Task<BarLineChartDto> GetBarLineChartAsync(string data, DateOnly periodBegin, DateOnly periodEnd)
    {
      try
      {
        BarLineChart barLineChart = new() { };


        if (data.Trim().ToLower() == "orders")
          barLineChart = await this._chartPersistence.GetOrdersBarLineChartAsync(periodBegin, periodEnd);
        else if (data.Trim().ToLower() == "revenue")
          barLineChart = await this._chartPersistence.GetRevenueBarLineChartAsync(periodBegin, periodEnd);
        else
          throw new Exception("Tipo de dado inválido");

        return _mapper.Map<BarLineChartDto>(barLineChart);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
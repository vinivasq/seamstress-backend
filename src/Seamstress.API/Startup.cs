using Seamstress.Application;
using Seamstress.Application.Contracts;
using Seamstress.Persistence;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Seamstress.API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<SeamstressContext>(
        context => context.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING")));

      services.AddControllers().AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
      });

      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
      services.AddScoped<IGeneralPersistence, GeneralPersistence>();
      services.AddScoped<IOrderService, OrderService>();
      services.AddScoped<IOrderPersistence, OrderPersistence>();
      services.AddScoped<ICustomerService, CustomerService>();
      services.AddScoped<ICustomerPersistence, CustomerPersistence>();
      services.AddScoped<IItemService, ItemService>();
      services.AddScoped<IItemPersistence, ItemPersistence>();
      services.AddScoped<IItemOrderService, ItemOrderService>();
      services.AddScoped<IItemOrderPersistence, ItemOrderPersistence>();
      services.AddScoped<IColorService, ColorService>();
      services.AddScoped<IColorPersistence, ColorPersistence>();
      services.AddScoped<IFabricService, FabricService>();
      services.AddScoped<IFabricPersistence, FabricPersistence>();
      services.AddScoped<ISetService, SetService>();
      services.AddScoped<ISetPersistence, SetPersistence>();
      services.AddScoped<ISizeService, SizeService>();
      services.AddScoped<ISizePersistence, SizePersistence>();
      services.AddScoped<IItemColorPersistence, ItemColorPersistence>();
      services.AddScoped<IItemFabricPersistence, ItemFabricPersistence>();
      services.AddScoped<IImageService, ImageService>();
      services.AddScoped<IAzureBlobService, AzureBlobService>();
      services.AddCors();
      services.AddSwaggerGen(c =>
      {
        c.CustomSchemaIds(type => type.ToString());
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Seamstress.API v1"));
      }

      app.UseHttpsRedirection();

      app.UseStaticFiles();

      app.UseRouting();

      app.UseCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
  }
}
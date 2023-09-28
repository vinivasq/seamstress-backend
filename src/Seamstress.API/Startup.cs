using Seamstress.Application;
using Seamstress.Application.Contracts;
using Seamstress.Persistence;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.Extensions.FileProviders;

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
        context => context.UseNpgsql("host=containers-us-west-43.railway.app;port=7916;username=postgres;password=Ip9wIRrd9a3Bk5kOPaGD;database=railway"));

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
      services.AddCors();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Seamstress.API", Version = "v1" });
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

      app.UseRouting();

      app.UseAuthorization();

      app.UseCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

      app.UseStaticFiles();

      // app.UseStaticFiles(new StaticFileOptions()
      // {
      //   FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
      //   RequestPath = new PathString("/Resources")
      // });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
  }
}
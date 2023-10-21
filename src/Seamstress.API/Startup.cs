using Seamstress.Application;
using Seamstress.Application.Contracts;
using Seamstress.Persistence;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Seamstress.API.Helpers;
using Seamstress.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

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
        context => context.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING")!));

      services.AddIdentityCore<User>(options =>
      {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
      })
        .AddRoles<Role>()
        .AddRoleManager<RoleManager<Role>>()
        .AddSignInManager<SignInManager<User>>()
        .AddRoleValidator<RoleValidator<Role>>()
        .AddEntityFrameworkStores<SeamstressContext>()
        .AddDefaultTokenProviders();

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TOKEN_KEY")!)),
            ValidateIssuer = false,
            ValidateAudience = false
          };
        });

      services.AddControllers().AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
      services.AddScoped<IUserPersistence, UserPersistence>();
      services.AddScoped<IUserService, UserService>();
      services.AddScoped<ITokenService, TokenService>();

      services.AddCors(options =>
        {
          // this defines a CORS policy called "default"
          options.AddPolicy("default", policy =>
          {
            policy.WithOrigins("https://seamstress-frontend-production.up.railway.app/*")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
          });
        });
      services.AddSwaggerGen(c =>
      {
        c.CustomSchemaIds(type => type.ToString());
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Description = @"JWT Authorization header usando Bearer.
                        Entre com 'Bearer' [espaço] token
                        Exemplo: 'Bearer 12334abcde'",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header
            },
            new List<string>()
          }
        });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // if (env.IsDevelopment())
      // {
      //   app.UseDeveloperExceptionPage();
      //   app.UseSwagger();
      //   app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Seamstress.API v1"));
      // }

      app.UseHttpsRedirection();

      app.UseStaticFiles();

      app.UseRouting();

      app.UseCors("default");

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      var scope = app.ApplicationServices.CreateScope();
      await MigrationsHelper.ManageDataAsync(scope.ServiceProvider);

      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
  }
}
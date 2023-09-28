namespace Seamstress.API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      WebApplication.CreateBuilder(new WebApplicationOptions()
      {

        Args = args,

        ContentRootPath = "/app/out",

        WebRootPath = "wwwroot",

      });
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();
        });

  }
}
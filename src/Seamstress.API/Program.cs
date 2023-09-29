namespace Seamstress.API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
      {
        Args = args,

        ContentRootPath = "/app/out",

        WebRootPath = "wwwroot",
      });

      var app = builder.Build();

      app.Run();
    }


  }
}
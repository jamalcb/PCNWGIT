using Microsoft.AspNetCore;
using NLog.Web;

namespace PCNW
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    //logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .UseNLog()
                .UseStartup<Startup>();
    }
}
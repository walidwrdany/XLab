using Serilog;
using XLab.Web.ExtensionMethod;

namespace XLab.Web;

public class Program
{
    public static async Task Main(string[] args) =>
        await CreateHostBuilder(args).Build()
            .AppInitialize()
            .RunAsync();


    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .UseSerilog((hostingContext, loggerConfig) => loggerConfig.ReadFrom.Configuration(hostingContext.Configuration));
}
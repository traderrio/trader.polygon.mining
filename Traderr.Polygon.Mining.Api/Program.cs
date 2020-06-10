using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Filters;
using Traderr.Polygon.Mining.Api.Polygon;

namespace Traderr.Polygon.Mining.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{env ?? "Development"}.json", optional: true)
                .Build();

            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            var pathToLogFolder = isLinux ? $"../../logs/{env}.polygon-mining/" : $"Logs/{env}/";

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Logger(l => l.Filter.ByExcluding(Matching.FromSource<IPolygonApiClient>())
                    .WriteTo.Async(a =>
                        a.File(pathToLogFolder + "main.log", rollingInterval:RollingInterval.Day))
                )
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(Matching.FromSource<IPolygonApiClient>())
                    .WriteTo.Async(a =>
                        a.File(pathToLogFolder + "polygon-client.log", rollingInterval: RollingInterval.Day))
                )
                .CreateLogger();


            try
            {
                Log.Information("Starting Polygon Mining...");
                CreateHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
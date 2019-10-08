using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Trader.Polygon.Api.Common;
using Trader.Polygon.Api.Logger;
using Trader.Polygon.Api.MessageHandlers.ServerSpecific;
using Trader.Polygon.Api.Receivers.Interfaces;

namespace Trader.Polygon.Api
{
	public class Program
	{

		public static int Main(string[] args)
		{
			var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env ?? "Development"}.json", optional: true)
				.Build();

			var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
			var pathToLogFolder = isLinux
				? $"/var/log/{env}.polygon.api/"
				: $"Logs/";

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.Enrich.FromLogContext()
				
				.WriteTo.Logger(l => l
					.Filter.ByIncludingOnly(Matching.FromSource<IStocksStreamReceiver>())
					.Filter.ByIncludingOnly(LoggerFilters.Time)
					.WriteTo.Async(a =>
						a.File(pathToLogFolder + "polygon-streaming.log",
							rollingInterval:RollingInterval.Day,
							retainedFileCountLimit: 30)))
				
				.WriteTo.Logger(l => l
					.Filter.ByIncludingOnly(Matching.FromSource<StockServerMessageHandler>())
					.Filter.ByIncludingOnly(LoggerFilters.Time)
					.WriteTo.Async(a =>
						a.File(pathToLogFolder + "polygon-streaming-stocks.log",
							rollingInterval:RollingInterval.Day,
							retainedFileCountLimit: 30)))
				
				.WriteTo.Logger(l => l.Filter.ByIncludingOnly(Matching.FromSource<IPipelineProcessorLoggerCategory>())
					.WriteTo.Async(a =>
						a.File(pathToLogFolder + "polygon-saver-strategies.log", 
							rollingInterval:RollingInterval.Day,
							retainedFileCountLimit: 3)))
				.WriteTo.Async(
					a => a.File(pathToLogFolder + "polygon.log", LogEventLevel.Warning))
				.CreateLogger();

			try
			{

				CreateWebHostBuilder(args).Build().Run();
				Log.Information("Polygon is running...");

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

		

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, config) =>
				{
					// Configure the app here.
				})
				.UseStartup<Startup>()
				.UseIISIntegration()
				.UseDefaultServiceProvider(options => options.ValidateScopes = false)
				.CaptureStartupErrors(true)
				.UseUrls("http://localhost:8000/")
				.UseSerilog();
		}
		
	}
}
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace CodeFest.Api
{
	public static class Program
	{
		public static readonly string Namespace = typeof(Program).Namespace;
		public static readonly string AppName = Namespace.Split('.')[Namespace.Split('.').Length - 2];

		public static int Main(string[] args)
		{
			var configuration = GetConfiguration();

			Log.Logger = CreateSerilogLogger(configuration);

			try
			{
				Log.Information("Configuring web host ({ApplicationContext})...", AppName);
				var host = BuildWebHost(configuration, args);		

				Log.Information("Starting web host ({ApplicationContext})...", AppName);
				host.Run();

				return 0;
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.CaptureStartupErrors(false)
				.UseStartup<Startup>()
				.UseContentRoot(Directory.GetCurrentDirectory())
				//.ConfigureAppConfiguration((host, config) =>
				//{
				//	if (host.HostingEnvironment.IsDevelopment())
				//		config.AddUserSecrets<Startup>();
				//})
				.UseWebRoot("Assets")
				.UseConfiguration(configuration)
				.UseSerilog()
				.Build();

		private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
		{
			return new LoggerConfiguration()
				.MinimumLevel.Information()
				.Enrich.WithProperty("ApplicationContext", AppName)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.ReadFrom.Configuration(configuration)
				.CreateLogger();
		}

		private static IConfiguration GetConfiguration()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables();

			return builder.Build();
		}
	}
}

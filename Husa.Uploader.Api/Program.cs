namespace Husa.Uploader.Api
{
    using System;
    using Figgle;
    using Husa.Uploader.Api.ServiceBus;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(FiggleFonts.Standard.Render("RP STARTED!"));
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, configuration) =>
                {
                    configuration
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                    if (host.HostingEnvironment.IsDevelopment())
                    {
                        configuration.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    }

                    configuration
                        .AddEnvironmentVariables()
                        .AddUserSecrets<Startup>();
                })
                .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((host, services) =>
                {
                    var featureFlags = host.Configuration.GetSection("Application:FeatureFlags").Get<FeatureFlags>();
                    if (featureFlags.EnableBusHandlers)
                    {
                        services.AddHostedService<Worker>();
                    }
                });
    }
}

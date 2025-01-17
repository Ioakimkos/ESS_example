using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ESS_example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Serilog.Debugging.SelfLog.Enable(Console.Out);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                // for use appsettings
               // .ReadFrom.Configuration(configuration)
                .WriteTo.Elasticsearch(new[] { new Uri("http://localhost:9200") }, opts =>
                {
                    opts.DataStream = new DataStreamName("logs", "ESS-example", "demo");
                    opts.BootstrapMethod = BootstrapMethod.Failure;
                    opts.ConfigureChannel = channelOpts =>
                    {
                        channelOpts.BufferOptions = new BufferOptions
                        {
                            ExportMaxConcurrency = 10
                        };
                    };
                })
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting application...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
    }
}

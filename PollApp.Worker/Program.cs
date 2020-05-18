using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PollApp.Storage.Cosmos;
using System;

namespace PollApp.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    Console.WriteLine($"Connection string {hostContext.Configuration["Cosmos:ConnectionString"]}");
                    services.AddCosmosStorage(hostContext.Configuration["Cosmos:ConnectionString"]);
                    services.AddApplicationInsightsTelemetryWorkerService();
                });
    }
}

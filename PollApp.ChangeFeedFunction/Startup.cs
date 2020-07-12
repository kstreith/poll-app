using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PollApp.Storage.Cosmos;
using System;
using System.Linq;

[assembly: FunctionsStartup(typeof(PollApp.ChangeFeedFunction.Startup))]
namespace PollApp.ChangeFeedFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            //var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));
            //var config3 = descriptor?.ImplementationFactory.Invoke(null) as IConfigurationSection;
            //var config2 = descriptor?.ImplementationInstance as IConfigurationRoot;
            var services = builder.Services;
            services.AddCosmosStorage(config["Cosmos:ConnectionString"]);
            services.AddSingleton<CosmosPollTabulation>();
        }
    }
}

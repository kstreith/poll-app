using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace PollApp.Storage.Cosmos
{
    public static class DependencyExtensions
    {
        public static void AddCosmosStorage(this IServiceCollection services, string connectionString)
        {
            var cosmosClient = new CosmosClient(connectionString);
            services.AddSingleton(cosmosClient);
            services.AddTransient<IPollStorage, CosmosPollDocumentStorage>();
        }
    }
}

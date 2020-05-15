using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PollApp.Storage.Cosmos
{
    public class CosmosPollTabulation : IPollTabulation
    {
        private readonly CosmosClient _cosmosClient;

        public CosmosPollTabulation(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        private async Task<Container> Initialize()
        {
            Database db = await _cosmosClient.CreateDatabaseIfNotExistsAsync("PollDb");
            Container container = await db.CreateContainerIfNotExistsAsync(
                new ContainerProperties { Id = "PollData", DefaultTimeToLive = -1, PartitionKeyPath = "/partitionKey" });
            Container leaseContainer = await db.CreateContainerIfNotExistsAsync(
                new ContainerProperties { Id = "PollLeases", PartitionKeyPath = "/id" });
            return container;
        }

        public async Task RunPollTabulationProcess()
        {
            await Initialize();
            Container leaseContainer = _cosmosClient.GetContainer("PollDb", "PollLeases");
            Container monitoredContainer = _cosmosClient.GetContainer("PollDb", "PollData");
            ChangeFeedProcessor changeFeedProcessor = monitoredContainer
                .GetChangeFeedProcessorBuilder<Object>("changeFeedBasic", HandleChangesAsync)
                    .WithInstanceName("consoleHost")
                    .WithLeaseContainer(leaseContainer)
                    .Build();
            await changeFeedProcessor.StartAsync();
        }

        static async Task HandleChangesAsync(IReadOnlyCollection<Object> changes, CancellationToken cancellationToken)
        {
            foreach (var item in changes)
            {
                //Console.WriteLine($"\tDetected operation for item with id {item.id}, created at {item.creationTime}.");
                // Simulate work
                await Task.Delay(1);
            }
        }
    }
}

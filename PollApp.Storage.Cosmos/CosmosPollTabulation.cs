using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                .GetChangeFeedProcessorBuilder<JObject>("changeFeedBasic", HandleChangesAsync)
                    .WithInstanceName("consoleHost")
                    .WithLeaseContainer(leaseContainer)
                    .WithStartTime(DateTime.MinValue.ToUniversalTime())
                    .Build();
            await changeFeedProcessor.StartAsync();
        }

        async Task HandleChangesAsync(IReadOnlyCollection<JObject> changes, CancellationToken cancellationToken)
        {
            Container pollContainer = _cosmosClient.GetContainer("PollDb", "PollData");
            var pollResponseJObject = changes.Where(item => item.ContainsKey("Type") && item["Type"].ToString() == nameof(PollResponseDocument).ToLowerInvariant());
            var pollResponses = pollResponseJObject.Select(jObject => jObject.ToObject<PollResponseDocument>()).ToList();
            foreach (var pollResponse in pollResponses)
            {
                var pollId = pollResponse.PartitionKey;
                var pollAnswerId = pollResponse.PollAnswerId;
                var pollResultDocumentId = new DocumentId(pollId, pollId, nameof(PollResultDocument));
                PollResultDocument pollResult = null;
                var newResult = false;
                try
                {
                    pollResult = (await pollContainer.ReadItemAsync<PollResultDocument>(pollResultDocumentId.Id, new PartitionKey(pollResultDocumentId.PartitionKey))).Resource;
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    newResult = true;
                    pollResult = new PollResultDocument(pollId);
                }
                if (pollResult.PossibleAnswers.ContainsKey(pollAnswerId))
                {
                    pollResult.PossibleAnswers[pollAnswerId] = pollResult.PossibleAnswers[pollAnswerId] + 1;
                }
                else
                {
                    pollResult.PossibleAnswers.Add(pollAnswerId, 0);
                }
                if (newResult)
                {
                    await pollContainer.CreateItemAsync(pollResult);
                }
                else
                {
                    await pollContainer.ReplaceItemAsync(pollResult, pollResultDocumentId.Id, new PartitionKey(pollResultDocumentId.PartitionKey));
                }
                //Console.WriteLine($"\tProcessing poll response of {item.id}, created at {item.creationTime}.");
                // Simulate work
                //await Task.Delay(1);
            }
        }
    }
}

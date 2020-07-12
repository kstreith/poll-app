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
            Database db = await _cosmosClient.CreateDatabaseIfNotExistsAsync("PollDb", 400);
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

        public async Task RunTabulation(IReadOnlyCollection<JObject> changes)
        {
            var allChangesCount = changes.Count;
            var pollResponseJObject = changes.Where(item => item.ContainsKey("Type") && item["Type"].ToString() == nameof(PollResponseDocument).ToLowerInvariant());
            var pollResponses = pollResponseJObject.Select(jObject => jObject.ToObject<PollResponseDocument>()).ToList();
            var pollResponsesCount = pollResponses.Count;
            Container pollContainer = _cosmosClient.GetContainer("PollDb", "PollData");
            foreach (var pollResponse in pollResponses)
            {
                var existingPollResult = await GetExistingPollResults(pollContainer, pollResponse.PartitionKey);
                var updatedPollResults = CalculateUpdatedResults(existingPollResult, pollResponse);
                await SavePollResult(pollContainer, updatedPollResults);
            }
        }

        async Task HandleChangesAsync(IReadOnlyCollection<JObject> changes, CancellationToken cancellationToken)
        {
            await RunTabulation(changes);
        }

        private static async Task<PollResultDocument> GetExistingPollResults(Container pollContainer, string pollId)
        {
            var pollResultDocumentId = new DocumentId(pollId, pollId, nameof(PollResultDocument));
            try
            {
                var existingPollResult = (await pollContainer.ReadItemAsync<PollResultDocument>(pollResultDocumentId.Id, new PartitionKey(pollResultDocumentId.PartitionKey))).Resource;
                return existingPollResult;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        private static async Task SavePollResult(Container pollContainer, PollResultDocument pollResult)
        {
            if (pollResult.ETag == null)
            {
                await pollContainer.CreateItemAsync(pollResult);
            }
            else
            {
                await pollContainer.ReplaceItemAsync(pollResult, pollResult.Id, new PartitionKey(pollResult.PartitionKey), new ItemRequestOptions { IfMatchEtag = pollResult.ETag });
            }
        }

        private static PollResultDocument CalculateUpdatedResults(PollResultDocument pollResult, PollResponseDocument pollResponse)
        {
            pollResult = pollResult ?? new PollResultDocument(pollResponse.PartitionKey);
            var pollAnswerId = pollResponse.PollAnswerId;
            if (pollResult.PossibleAnswers.ContainsKey(pollResponse.PollAnswerId))
            {
                pollResult.PossibleAnswers[pollAnswerId] = pollResult.PossibleAnswers[pollAnswerId] + 1;
            }
            else
            {
                pollResult.PossibleAnswers.Add(pollAnswerId, 1);
            }
            return pollResult;
        }
    }
}

using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PollApp.Storage.Cosmos
{
    public class CosmosPollDocumentStorage : IPollDocumentStorage
    {
        private readonly CosmosClient _cosmosClient;

        public CosmosPollDocumentStorage(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        private async Task<Container> Initialize()
        {
            Database db = await _cosmosClient.CreateDatabaseIfNotExistsAsync("PollDb");
            Container container = await db.CreateContainerIfNotExistsAsync(
                new ContainerProperties { Id = "PollData", DefaultTimeToLive = -1, PartitionKeyPath = "/partitionKey" });
            return container;
        }

        public async Task CreatePoll(string id, Poll pollDocument)
        {
            var container = await Initialize();
            await container.CreateItemAsync(new PollDocument(id)
            {
                Name = pollDocument.Name,
                Question = pollDocument.Question,
                PossibleAnswers = pollDocument.PossibleAnswers.Select(answer => new PollDocument.PollAnswer { Id = answer.Id, Answer = answer.Answer }).ToList()
            });
        }

        public async Task<Poll> GetPoll(string id)
        {
            var container = await Initialize();
            throw new NotImplementedException();
        }
    }
}

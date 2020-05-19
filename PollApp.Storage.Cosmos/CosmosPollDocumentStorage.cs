using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PollApp.Storage.Cosmos
{
    public class CosmosPollDocumentStorage : IPollStorage
    {
        private readonly CosmosClient _cosmosClient;

        public CosmosPollDocumentStorage(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        private async Task<Container> Initialize()
        {
            Database db = await _cosmosClient.CreateDatabaseIfNotExistsAsync("PollDb", 400);
            Container container = await db.CreateContainerIfNotExistsAsync(
                new ContainerProperties { Id = "PollData", DefaultTimeToLive = -1, PartitionKeyPath = "/partitionKey" });
            return container;
        }

        public async Task CreatePoll(string id, Poll pollDocument)
        {
            var container = await Initialize();
            await container.CreateItemAsync(new PollDocument(id)
            {
                Question = pollDocument.Question,
                PossibleAnswers = pollDocument.PossibleAnswers.Select(answer => new PollDocument.PollAnswer { Id = answer.Id, Answer = answer.Answer }).ToList()
            });
        }

        public async Task<Poll> GetPoll(string id)
        {
            var container = await Initialize();
            var pollDocumentId = new DocumentId(id, id, nameof(PollDocument));
            PollDocument pollDocument = null;
            try
            {
                pollDocument = (await container.ReadItemAsync<PollDocument>(pollDocumentId.Id, new PartitionKey(pollDocumentId.PartitionKey))).Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            var pollResultDocumentId = new DocumentId(id, id, nameof(PollResultDocument));
            PollResultDocument pollResultDocument = null;
            try
            {
                pollResultDocument = (await container.ReadItemAsync<PollResultDocument>(pollResultDocumentId.Id, new PartitionKey(pollResultDocumentId.PartitionKey))).Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
            }
            var pollAnswerResponses = pollResultDocument?.PossibleAnswers;
            return new Poll
            {
                Question = pollDocument.Question,
                PossibleAnswers = pollDocument.PossibleAnswers.Select(answer => {
                    var responseCount = 0;
                    if (pollAnswerResponses?.ContainsKey(answer.Id) == true)
                    {
                        responseCount = pollAnswerResponses[answer.Id];
                    }
                    return new PollAnswer(answer.Id, answer.Answer, responseCount);
                }).ToList()
            };
        }

        public async Task RecordAnswer(string id, PollAnswer pollAnswer)
        {
            var container = await Initialize();
            var responseId = Guid.NewGuid().ToString();
            var pollResponse = new PollResponseDocument(responseId, id)
            {
                PollAnswerId = pollAnswer.Id
            };
            await container.CreateItemAsync(pollResponse);
        }
    }
}

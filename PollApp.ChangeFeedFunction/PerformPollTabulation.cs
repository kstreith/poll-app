using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;

namespace PollApp.ChangeFeedFunction
{
    public class PerformPollTabulation
    {
        private readonly PollApp.Storage.Cosmos.CosmosPollTabulation _cosmosPollTabulation;

        public PerformPollTabulation(PollApp.Storage.Cosmos.CosmosPollTabulation cosmosPollTabulation)
        {
            _cosmosPollTabulation = cosmosPollTabulation;
        }

        [FunctionName("PerformPollTabulation")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "PollDb",
            collectionName: "PollData",
            ConnectionStringSetting = "Cosmos:ConnectionString",
            LeaseCollectionName = "PollAzFunctionLease",
            StartFromBeginning = true,
            FeedPollDelay = 3000)]IReadOnlyList<Microsoft.Azure.Documents.Document> changes)
        {
            var jObjects = changes.Select(o => JObject.FromObject(o)).ToList();
            await _cosmosPollTabulation.RunTabulation(jObjects);
        }
    }
}

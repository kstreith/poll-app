using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PollApp.Storage.Cosmos;

namespace PollApp.ChangeFeedFunction
{
    public class PerformPollTabulation
    {
        private readonly PollApp.Storage.Cosmos.CosmosPollTabulation _cosmosPollTabulation;
        private readonly ILogger<PerformPollTabulation> _logger;

        public PerformPollTabulation(PollApp.Storage.Cosmos.CosmosPollTabulation cosmosPollTabulation, ILogger<PerformPollTabulation> logger)
        {
            _cosmosPollTabulation = cosmosPollTabulation;
            _logger = logger;
        }

        [FunctionName("PerformPollTabulation")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "PollDb",
            collectionName: "PollData",
            ConnectionStringSetting = "Cosmos:ConnectionString",
            LeaseCollectionName = "PollAzFunctionLease",
            StartFromBeginning = true,
            FeedPollDelay = 1000)]IReadOnlyList<Microsoft.Azure.Documents.Document> changes)
        {
            var jObjects = changes.Select(o => JObject.FromObject(o)).ToList();
            var pollResponseCount = jObjects.Where(item => item.ContainsKey("Type") && item["Type"].ToString() == nameof(PollResponseDocument).ToLowerInvariant()).Count();
            _logger.LogInformation("PerformPollTabulation Executed With {TotalDocumentCount} and {PollResponseDocumentCount}", jObjects.Count, pollResponseCount);
            await _cosmosPollTabulation.RunTabulation(jObjects);
        }
    }
}

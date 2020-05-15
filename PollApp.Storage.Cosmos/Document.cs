using Newtonsoft.Json;

namespace PollApp
{
    public class Document
    {
        private readonly DocumentId _documentId;

        public Document(DocumentId documentId)
        {
            _documentId = documentId;
        }

        [JsonProperty("id")]
        public string Id => _documentId.Id;

        [JsonProperty("partitionKey")]
        public string PartitionKey => _documentId.PartitionKey;
    }
}

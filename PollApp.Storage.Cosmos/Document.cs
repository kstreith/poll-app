using Newtonsoft.Json;

namespace PollApp
{
    public class Document
    {
        private readonly DocumentId _documentId;

        public Document(DocumentId documentId)
        {
            PartitionKey = documentId?.PartitionKey;
            Id = documentId?.Id;
            Type = documentId?.Type;
            _documentId = documentId;
        }

        [JsonProperty("id")]
        internal string Id;

        [JsonProperty("partitionKey")]
        internal string PartitionKey;

        [JsonProperty("Type")]
        internal string Type;
    }
}

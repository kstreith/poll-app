namespace PollApp
{
    public class DocumentId
    {
        private readonly string _resourceId;
        private readonly string _partitionKey;
        private readonly string _type;

        public DocumentId(string resourceId, string partitionKey, string type)
        {
            _resourceId = resourceId.ToLowerInvariant();
            _partitionKey = partitionKey.ToLowerInvariant();
            _type = type.ToLowerInvariant();
        }

        public string Id => $"{_resourceId}|{_type}";

        public string PartitionKey => _partitionKey;
    }
}

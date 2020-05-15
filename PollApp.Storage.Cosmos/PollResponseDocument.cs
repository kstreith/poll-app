using Newtonsoft.Json;

namespace PollApp.Storage.Cosmos
{
    public class PollResponseDocument : Document
    {
        [JsonConstructor]
        private PollResponseDocument() : base(null) {}
        public PollResponseDocument(string responseId, string pollId) : base(new DocumentId(responseId, pollId, nameof(PollResponseDocument)))
        {
        }

        public string PollAnswerId { get; set; }
    }
}

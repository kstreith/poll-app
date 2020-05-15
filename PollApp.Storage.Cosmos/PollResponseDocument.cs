namespace PollApp.Storage.Cosmos
{
    public class PollResponseDocument : Document
    {
        public PollResponseDocument(string responseId, string pollId) : base(new DocumentId(responseId, pollId, nameof(PollResponseDocument)))
        {
        }

        public string PollAnswerId { get; set; }
    }
}

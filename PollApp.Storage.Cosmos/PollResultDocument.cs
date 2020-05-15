using System.Collections.Generic;

namespace PollApp.Storage.Cosmos
{
    public class PollResultDocument : Document
    {
        public class PollAnswer
        {
            public string Id { get; set; }

            public int ResponseCount { get; set; }
        }

        public PollResultDocument(string pollId) : base(new DocumentId(pollId, pollId, nameof(PollResultDocument)))
        {
        }

        public List<PollAnswer> PossibleAnswers { get; set; }
    }
}

using System.Collections.Generic;

namespace PollApp.Storage.Cosmos
{
    public class PollDocument : Document
    {
        public class PollAnswer
        {
            public string Id { get; set; }

            public string Answer { get; set; }
        }

        public PollDocument(string id) : base(new DocumentId(id, id, nameof(PollDocument)))
        {
        }

        public string Question { get; set; }

        public List<PollAnswer> PossibleAnswers { get; set; }
    }
}

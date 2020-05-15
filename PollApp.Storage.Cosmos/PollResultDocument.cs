﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace PollApp.Storage.Cosmos
{
    public class PollResultDocument : Document
    {
        [JsonConstructor]
        private PollResultDocument() : base(null) { }

        public PollResultDocument(string pollId) : base(new DocumentId(pollId, pollId, nameof(PollResultDocument)))
        {
        }

        public Dictionary<string, int> PossibleAnswers { get; set; } = new Dictionary<string, int>();
    }
}

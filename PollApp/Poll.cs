using System;
using System.Collections.Generic;
using System.Linq;

namespace PollApp
{
    public class Poll
    {
        public string Id { get; set; }

        public string Question { get; set; }

        public void SetAnswers(List<string> answers)
        {
            PossibleAnswers = answers.Select(x => new PollAnswer
            {
                Id = Guid.NewGuid().ToString(),
                Answer = x
            }).ToList();
        }

        public List<PollAnswer> PossibleAnswers { get; set; }
    }
}

using System.Collections.Generic;

namespace PollApp
{
    public class Poll
    {
        public string Name { get; set; }

        public string Question { get; set; }

        public List<PollAnswer> PossibleAnswers { get; set; }
    }
}

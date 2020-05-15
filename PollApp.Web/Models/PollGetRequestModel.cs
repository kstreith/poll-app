using System.Collections.Generic;

namespace PollApp.Web.Models
{
    public class PollGetRequestModel
    {
        public class PollAnswer
        {
            public string Id { get; set; }
            public string Answer { get; set; }
            public int ResponseCount { get; set; }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Question { get; set; }
        public List<PollAnswer> PossibleAnswers { get; set; }
    }
}

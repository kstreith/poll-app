using System.Collections.Generic;

namespace PollApp.Web.Models
{
    public class PollCreateRequestModel
    {
        public string Id { get; set; }

        public string Question { get; set; }

        public List<string> PossibleAnswers { get; set; }
    }
}

namespace PollApp
{
    public class PollAnswer
    {
        public PollAnswer() { }
        public PollAnswer(string id, string answer, int responseCount)
        {
            Id = id;
            Answer = answer;
            ResponseCount = responseCount;
        }

        public string Id { get; set; }

        public string Answer { get; set; }

        public int ResponseCount { get; }
    }
}

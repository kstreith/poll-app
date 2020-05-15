using System.Threading.Tasks;

namespace PollApp
{
    public interface IPollStorage
    {
        Task CreatePoll(string id, Poll pollDocument);

        Task<Poll> GetPoll(string id);

        Task RecordAnswer(Poll pollDocument, PollAnswer pollAnswer);
    }
}

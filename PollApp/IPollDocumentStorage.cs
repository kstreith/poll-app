using System.Threading.Tasks;

namespace PollApp
{
    public interface IPollDocumentStorage
    {
        Task CreatePoll(string id, Poll pollDocument);

        Task<Poll> GetPoll(string id);

        Task RecordAnswer(Poll pollDocument, PollAnswer pollAnswer);
    }
}

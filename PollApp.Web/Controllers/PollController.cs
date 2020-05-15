using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PollApp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollController : ControllerBase
    {
        private readonly IPollStorage _pollDocumentStorage;

        public PollController(IPollStorage pollDocumentStorage)
        {
            _pollDocumentStorage = pollDocumentStorage;
        }

        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Default
        [HttpPost]
        public async Task Post([FromBody] string value)
        {
            var pollId = Guid.NewGuid().ToString();
            await _pollDocumentStorage.CreatePoll(pollId, new Poll
            {
                Name = "Test",
                Question = "Have you used NoSql solutions before?",
                PossibleAnswers = new List<PollAnswer> {
                    new PollAnswer
                    {
                        Id = "Yes",
                        Answer = "Yes"
                    },
                    new PollAnswer
                    {
                        Id = "No",
                        Answer = "No"
                    }
                }
            });
        }
    }
}
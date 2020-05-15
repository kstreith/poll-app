﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PollApp.Web.Models;

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
        public async Task<ActionResult<PollGetRequestModel>> Get(string id)
        {
            var poll = await _pollDocumentStorage.GetPoll(id);
            if (poll == null)
            {
                return new NotFoundResult();
            }
            return new ActionResult<PollGetRequestModel>(new PollGetRequestModel
            {
                Id = id,
                Name = poll.Name,
                Question = poll.Question,
                PossibleAnswers = poll.PossibleAnswers.Select(x => new PollGetRequestModel.PollAnswer
                {
                    Id = x.Id,
                    Answer = x.Answer,
                    ResponseCount = x.ResponseCount
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<CreatedAtActionResult> Post([FromBody] PollCreateRequestModel pollCreateRequest)
        {
            var pollId = Guid.NewGuid().ToString();
            var poll = new Poll
            {
                Name = pollCreateRequest.Name,
                Question = pollCreateRequest.Question,
            };
            poll.SetAnswers(pollCreateRequest.PossibleAnswers);
            await _pollDocumentStorage.CreatePoll(pollId, poll);
            return new CreatedAtActionResult(nameof(Get), nameof(PollController), new { id = pollId }, null);
        }

        [HttpPost("{id}/answer/{answerId}")]
        public async Task<ActionResult> Post(string id, string answerId)
        {
            var poll = await _pollDocumentStorage.GetPoll(id);
            if (poll == null)
            {
                return new NotFoundResult();
            }
            var foundAnswer = poll.PossibleAnswers.SingleOrDefault(x => x.Id == answerId);
            if (foundAnswer == null)
            {
                return new NotFoundResult();
            }
            await _pollDocumentStorage.RecordAnswer(id, foundAnswer);
            return new OkResult();
        }
    }
}
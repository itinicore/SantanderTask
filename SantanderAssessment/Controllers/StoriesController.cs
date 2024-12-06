using MediatR;
using Microsoft.AspNetCore.Mvc;
using SantanderAssessment.Queries;

namespace SantanderAssessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly ILogger<StoriesController> _logger;
        private readonly IMediator _mediator;

        public StoriesController(ILogger<StoriesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetStoriesRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stories = await _mediator.Send(new GetStories.Query(request.Limit));
            return Ok(stories);
        }
    }
}

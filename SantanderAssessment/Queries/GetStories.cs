using MediatR;
using SantanderAssessment.Data;
using SantanderAssessment.Models;

namespace SantanderAssessment.Queries
{
    public static class GetStories
    {
        public class Query : IRequest<List<Story>>
        {
            public int Limit { get; private set; }

            public Query(int limit)
            {
                Limit = limit;
            }
        }

        internal class Handler : IRequestHandler<Query, List<Story>>
        {
            private readonly IStoriesStateService _storiesState;

            public Handler(IStoriesStateService storiesState)
            {
                _storiesState = storiesState;
            }

            public async Task<List<Story>> Handle(Query request, CancellationToken cancellationToken)
            {
                var stories = _storiesState.GetStories(request.Limit);
                return await Task.FromResult(stories);
            }
        }
    }
}

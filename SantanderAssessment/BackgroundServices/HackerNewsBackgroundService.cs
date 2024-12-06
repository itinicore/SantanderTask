using System.Collections.Concurrent;

namespace SantanderAssessment.BackgroundServices
{
    public class HackerNewsBackgroundService : BackgroundService
    {
        private readonly IHackerNewsObserver _hackerNewsObserver;
        private CancellationToken _stoppingToken;
        private readonly ILogger<HackerNewsBackgroundService> _logger;
        
        private readonly ConcurrentDictionary<int, (CancellationTokenSource TokenSource, Task Task)> _storyTasks =
            new ConcurrentDictionary<int, (CancellationTokenSource, Task)>();

        public HackerNewsBackgroundService(IHackerNewsObserver hackerNewsObserver, ILogger<HackerNewsBackgroundService> logger)
        {
            _hackerNewsObserver = hackerNewsObserver;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //return;
            _stoppingToken = stoppingToken;
            try
            {
                // Respond to stoppingToken cancellation
                stoppingToken.Register(() =>
                {
                    foreach (var (_, (tokenSource, _)) in _storyTasks)
                    {
                        tokenSource.Cancel(); // Cancel individual tasks
                    }
                });

                // Main execution logic
                await _hackerNewsObserver.ObserveNewsList(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background service execution.");
            }
        }

        public bool IsAlreadyListeningToStory(int id)
        {
            return _storyTasks.ContainsKey(id);
        }

        public void StartListeningSingleStory(int id)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken);
            var token = cts.Token;

            var task = Task.Run(async () =>
            {
                try
                {
                    await _hackerNewsObserver.ObserveSingleStory(id, token);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogTrace($"Task for story {id} was canceled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error observing story {id}.");
                }
            }, token);


            _storyTasks[id] = (cts, task);
            _logger.LogTrace($"Started listening for story {id} updates.");
        }

        public void StopListeningForStory(int id)
        {
            if (_storyTasks.TryRemove(id, out var taskInfo))
            {
                taskInfo.TokenSource.Cancel();
                taskInfo.TokenSource.Dispose();

                _logger.LogTrace($"Stopped listening for story {id} updates.");
            }
            else
            {
                _logger.LogWarning($"No active listener found for story {id}.");
            }
        }
    }
}

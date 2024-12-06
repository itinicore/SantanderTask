namespace SantanderAssessment.BackgroundServices
{
    public interface IHackerNewsObserver
    {
        Task ObserveNewsList(CancellationToken stoppingToken);
        Task ObserveSingleStory(int storyId, CancellationToken stoppingToken);
    }
}

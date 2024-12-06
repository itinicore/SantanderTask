using MediatR;
using SantanderAssessment.BackgroundServices;
using SantanderAssessment.Data;
using SantanderAssessment.Models;
using SantanderAssessment.Notifications;
using System.Linq;

namespace SantanderAssessment.NotificationHandlers
{
    public class StoriesListUpdatedNotificationHandler : INotificationHandler<StoriesListUpdatedNotification>
    {
        
        private readonly HackerNewsBackgroundService _hackerNewsBackgroundService;
        private readonly IStoriesStateService _storiesStateService;

        public StoriesListUpdatedNotificationHandler(HackerNewsBackgroundService hackerNewsBackgroundService, IStoriesStateService storiesStateService)
        {
            _hackerNewsBackgroundService = hackerNewsBackgroundService;
            _storiesStateService = storiesStateService;
        }


        public async Task Handle(StoriesListUpdatedNotification notification, CancellationToken cancellationToken)
        {
            

            switch (notification.Op)
            {
                case StoriesListUpdatedNotification.Operation.LoadList:
                    await LoadList(notification.StoryIds);
                    break;

                case StoriesListUpdatedNotification.Operation.AddElement:
                    AddElement(notification.StoryIds.First());
                    break;

                case StoriesListUpdatedNotification.Operation.RemoveElement:
                    RemoveElement(notification.StoryIndex);
                    break;

                case StoriesListUpdatedNotification.Operation.UpdateElement:
                    UpdateElement(notification.StoryIndex, notification.StoryIds.First());
                    break;


            }


        }

        private void UpdateElement(int index, int id)
        {
            
            RemoveElement(index);
            AddElement(id);
        }

        private void RemoveElement(int index)
        {

            var stories = _storiesStateService.GetStories();
            if (index > stories.Count)
            {
                return;
            }
            
            var story = stories[index];
            _storiesStateService.DeleteStory(story.Id);
            _hackerNewsBackgroundService.StopListeningForStory(story.Id);
        }

        private void AddElement(int newId)
        {
            _hackerNewsBackgroundService.StartListeningSingleStory(newId);
        }

        private async Task LoadList(List<int> storyIds)
        {
            var stories = _storiesStateService.GetStories();
            var noLongerBestStories = stories.Where(x => !storyIds.Contains(x.Id)).ToList();



            if (noLongerBestStories.Any())
            {
                _storiesStateService.DeleteStories(noLongerBestStories);
                foreach (var story in noLongerBestStories)
                {
                    _hackerNewsBackgroundService.StopListeningForStory(story.Id);
                }

            }

            while (storyIds.Any())
            {
                var batch = storyIds.Take(10).ToList();
                foreach (var storyId in batch)
                {
                    if (!_hackerNewsBackgroundService.IsAlreadyListeningToStory(storyId));
                    {
                        AddElement(storyId);
                    }
                }

                if (storyIds.Any())
                {
                    storyIds = storyIds.Skip(Math.Min(10, storyIds.Count)).ToList();
                }

                if (storyIds.Any())
                {
                    await Task.Delay(1000);
                }

            }
        }
    }
}

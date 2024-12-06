using MediatR;
using SantanderAssessment.Data;
using SantanderAssessment.Models;
using SantanderAssessment.Notifications;

namespace SantanderAssessment.NotificationHandlers
{
    public class StoryUpdatedNotificationHandler : INotificationHandler<StoryUpdatedNotification>
    {
        private readonly IStoriesStateService _stateService;

        public StoryUpdatedNotificationHandler(IStoriesStateService stateService)
        {
            _stateService = stateService;
        }

        public Task Handle(StoryUpdatedNotification notification, CancellationToken cancellationToken)
        {
            var storyId = notification.StoryId;
            if (notification.Data == null)
            {
                // Delete
                _stateService.DeleteStory(storyId);
            }
            else
            {
                if (_stateService.IsIdOnTheList(storyId))
                {
                    var story = _stateService.GetStory(storyId);
                    story.ApplyFirebaseUpdate(notification.Data);
                    _stateService.UpdateStory(story);
                }
                else
                {
                    var story = new Story
                    {
                        Id = storyId
                    };
                    story.ApplyFirebaseUpdate(notification.Data);
                    _stateService.AddStory(story);
                }
            }

            return Task.CompletedTask;
        }
    }
}

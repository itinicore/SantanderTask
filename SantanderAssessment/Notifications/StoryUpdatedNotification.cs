using MediatR;

namespace SantanderAssessment.Notifications
{
    public class StoryUpdatedNotification : INotification
    {
        public int StoryId { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public StoryUpdatedNotification(int storyId, Dictionary<string, object> data)
        {
            StoryId = storyId;
            Data = data;
        }
    }
}

using SantanderAssessment.Models;

namespace SantanderAssessment.Data
{
    public interface IStoriesRepository
    {
        List<Story> GetStories();
        void AddStory(Story story);
        void UpdateStory(Story story);
        void DeleteStory(Story story);
        void DeleteStories(List<Story> stories);
    }
}

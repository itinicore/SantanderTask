using SantanderAssessment.Models;

namespace SantanderAssessment.Data
{
    public interface IStoriesStateService
    {
        Story GetStory(int id);
        List<Story> GetStories();
        List<Story> GetStories(int limit);
        bool IsIdOnTheList(int id);
        void AddStory(Story story);
        void UpdateStory(Story story);
        void DeleteStory(Story story);
        void DeleteStories(List<Story> noLongerBestStories);
        void DeleteStory(int storyId);
    }
}

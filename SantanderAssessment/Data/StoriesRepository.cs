using LiteDB;
using SantanderAssessment.Models;

namespace SantanderAssessment.Data
{
    public class StoriesRepository : IStoriesRepository
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<Story> _stories;

        public StoriesRepository()
        {
            _database = new LiteDatabase("Stories.db");
            _stories = _database.GetCollection<Story>("stories");
        }

        public StoriesRepository(LiteDatabase database)
        {
            _database = database;
            _stories = _database.GetCollection<Story>("stories");
        }

        // Add Story
        public void AddStory(Story story)
        {
            _stories.Insert(story);
        }

        // Delete Story
        public void DeleteStory(Story story)
        {
            _stories.Delete(story.Id);
        }

        // Delete Multiple Stories
        public void DeleteStories(List<Story> storiesToDelete)
        {
            foreach (var story in storiesToDelete)
            {
                _stories.Delete(story.Id);
            }
        }

        // Get All Stories
        public List<Story> GetStories()
        {
            return _stories.FindAll().ToList();
        }


        // Update Story
        public void UpdateStory(Story story)
        {
            _stories.Update(story);
        }
    }

}

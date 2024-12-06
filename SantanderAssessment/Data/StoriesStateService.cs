using SantanderAssessment.Models;
using System.Collections.Concurrent;

namespace SantanderAssessment.Data
{
    public class StoriesStateService : IStoriesStateService
    {
        private readonly IStoriesRepository _storiesRepository;
        private readonly object _lock = new object(); // Lock object for synchronization
        private List<Story> _stories;

        public StoriesStateService(IStoriesRepository storiesRepository)
        {
            _storiesRepository = storiesRepository;
            _stories = new List<Story>();
        }

        private void SortStories()
        {
            _stories = _stories.OrderByDescending(x => x.Score).ToList();
        }

        public List<Story> GetStories()
        {
            lock (_lock)
            {
                if (_stories == null || !_stories.Any())
                {
                    _stories = _storiesRepository.GetStories();
                }
                return new List<Story>(_stories); // Return a copy to avoid external modifications
            }
        }

        public List<Story> GetStories(int limit)
        {
            lock (_lock)
            {
                return GetStories().Take(limit).ToList();
            }
        }

        public Story GetStory(int id)
        {
            lock (_lock)
            {
                return GetStories().FirstOrDefault(x => x.Id == id)
                    ?? throw new KeyNotFoundException($"Story with ID {id} not found.");
            }
        }

        public bool IsIdOnTheList(int id)
        {
            lock (_lock)
            {
                return GetStories().Any(x => x.Id == id);
            }
        }

        public void AddStory(Story story)
        {
            lock (_lock)
            {
                _stories.Add(story);
                _storiesRepository.AddStory(story);
                SortStories();
            }
        }

        public void UpdateStory(Story story)
        {
            lock (_lock)
            {
                var existingStory = GetStory(story.Id);
                if (existingStory == null)
                {
                    throw new KeyNotFoundException($"Story with ID {story.Id} not found.");
                }

                var index = _stories.IndexOf(existingStory);
                if (index >= 0)
                {
                    _stories[index] = story;
                }

                _storiesRepository.UpdateStory(story);
                SortStories();
            }
        }

        public void DeleteStory(Story story)
        {
            lock (_lock)
            {
                if (_stories.Remove(story))
                {
                    _storiesRepository.DeleteStory(story);
                    SortStories();
                }
            }
        }

        public void DeleteStories(List<Story> noLongerBestStories)
        {
            lock (_lock)
            {
                foreach (var story in noLongerBestStories)
                {
                    _stories.Remove(story);
                }
                _storiesRepository.DeleteStories(noLongerBestStories);
                SortStories();
            }
        }

        public void DeleteStory(int storyId)
        {
            lock (_lock)
            {
                var story = GetStory(storyId);
                DeleteStory(story);
                SortStories();
            }
        }
    }
}


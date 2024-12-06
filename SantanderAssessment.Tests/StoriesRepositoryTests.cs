using LiteDB;
using SantanderAssessment.Data;
using SantanderAssessment.Models;

namespace SantanderAssessment.Tests
{
    public class StoriesRepositoryTests
    {
        private StoriesRepository CreateInMemoryRepository()
        {
            // Użycie in-memory bazy LiteDB
            return new StoriesRepository(new LiteDatabase(new MemoryStream()));
        }

        [Fact]
        public void AddStory_ShouldAddStoryToDatabase()
        {
            // Arrange
            var repository = CreateInMemoryRepository();
            var story = new Story
            {
                Id = 1,
                Title = "Test Story",
                Uri = "https://example.com/story",
                PostedBy = "user1",
                Time = DateTime.UtcNow.ToString(),
                Score = 100,
                CommentCount = 10
            };

            // Act
            repository.AddStory(story);
            var stories = repository.GetStories();

            // Assert
            Assert.Single(stories);
            Assert.Equal("Test Story", stories[0].Title);
            Assert.Equal("https://example.com/story", stories[0].Uri);
            Assert.Equal("user1", stories[0].PostedBy);
            Assert.Equal(100, stories[0].Score);
            Assert.Equal(10, stories[0].CommentCount);
        }

        [Fact]
        public void GetStories_ShouldReturnAllStories()
        {
            // Arrange
            var repository = CreateInMemoryRepository();
            var story1 = new Story
            {
                Id = 1,
                Title = "Story 1",
                Uri = "https://example.com/story1",
                PostedBy = "user1",
                Time = DateTime.UtcNow.ToString(),
                Score = 150,
                CommentCount = 20
            };
            var story2 = new Story
            {
                Id = 2,
                Title = "Story 2",
                Uri = "https://example.com/story2",
                PostedBy = "user2",
                Time = DateTime.UtcNow.ToString(),
                Score = 200,
                CommentCount = 30
            };
            repository.AddStory(story1);
            repository.AddStory(story2);

            // Act
            var stories = repository.GetStories();

            // Assert
            Assert.Equal(2, stories.Count);
            Assert.Contains(stories, s => s.Title == "Story 1" && s.Score == 150);
            Assert.Contains(stories, s => s.Title == "Story 2" && s.Score == 200);
        }

        [Fact]
        public void UpdateStory_ShouldUpdateExistingStory()
        {
            // Arrange
            var repository = CreateInMemoryRepository();
            var story = new Story
            {
                Id = 1,
                Title = "Old Title",
                Uri = "https://olduri.com",
                PostedBy = "oldUser",
                Time = DateTime.UtcNow.ToString(),
                Score = 50,
                CommentCount = 5
            };
            repository.AddStory(story);

            // Act
            story.Title = "Updated Title";
            story.Uri = "https://updateduri.com";
            story.PostedBy = "updatedUser";
            story.Score = 100;
            story.CommentCount = 10;
            repository.UpdateStory(story);
            var updatedStory = repository.GetStories().Find(s => s.Id == 1);

            // Assert
            Assert.NotNull(updatedStory);
            Assert.Equal("Updated Title", updatedStory.Title);
            Assert.Equal("https://updateduri.com", updatedStory.Uri);
            Assert.Equal("updatedUser", updatedStory.PostedBy);
            Assert.Equal(100, updatedStory.Score);
            Assert.Equal(10, updatedStory.CommentCount);
        }

        [Fact]
        public void DeleteStory_ShouldRemoveStoryFromDatabase()
        {
            // Arrange
            var repository = CreateInMemoryRepository();
            var story = new Story
            {
                Id = 1,
                Title = "Story to Delete",
                Uri = "https://example.com/delete",
                PostedBy = "userToDelete",
                Time = DateTime.UtcNow.ToString(),
                Score = 50,
                CommentCount = 5
            };
            repository.AddStory(story);

            // Act
            repository.DeleteStory(story);
            var stories = repository.GetStories();

            // Assert
            Assert.Empty(stories);
        }

        [Fact]
        public void DeleteStories_ShouldRemoveMultipleStories()
        {
            // Arrange
            var repository = CreateInMemoryRepository();
            var story1 = new Story
            {
                Id = 1,
                Title = "Story 1",
                Uri = "https://example.com/story1",
                PostedBy = "user1",
                Time = DateTime.UtcNow.ToString(),
                Score = 100,
                CommentCount = 10
            };
            var story2 = new Story
            {
                Id = 2,
                Title = "Story 2",
                Uri = "https://example.com/story2",
                PostedBy = "user2",
                Time = DateTime.UtcNow.ToString(),
                Score = 200,
                CommentCount = 20
            };
            repository.AddStory(story1);
            repository.AddStory(story2);

            // Act
            repository.DeleteStories(new List<Story> { story1, story2 });
            var stories = repository.GetStories();

            // Assert
            Assert.Empty(stories);
        }
    }
}
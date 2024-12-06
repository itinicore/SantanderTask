using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SantanderAssessment.Models;

namespace SantanderAssessment.Tests
{
    public class StoryTests
    {
        [Fact]
        public void ApplyFirebaseUpdate_ShouldUpdateTitleAndUri()
        {
            // Arrange
            var story = new Story
            {
                Id = 1,
                Title = "Old Title",
                Uri = "https://olduri.com"
            };

            var updateData = new Dictionary<string, object>
        {
            { "title", "New Title" },
            { "url", "https://newuri.com" }
        };

            // Act
            story.ApplyFirebaseUpdate(updateData);

            // Assert
            Assert.Equal("New Title", story.Title);
            Assert.Equal("https://newuri.com", story.Uri);
        }

        [Fact]
        public void ApplyFirebaseUpdate_ShouldUpdatePostedByAndScore()
        {
            // Arrange
            var story = new Story
            {
                Id = 1,
                PostedBy = "oldUser",
                Score = 10
            };

            var updateData = new Dictionary<string, object>
        {
            { "by", "newUser" },
            { "score", 20 }
        };

            // Act
            story.ApplyFirebaseUpdate(updateData);

            // Assert
            Assert.Equal("newUser", story.PostedBy);
            Assert.Equal(20, story.Score);
        }

        [Fact]
        public void ApplyFirebaseUpdate_ShouldUpdateTimeWithUnixTimestamp()
        {
            // Arrange
            var story = new Story
            {
                Id = 1,
                Time = "2023-01-01T12:00:00"
            };

            var updateData = new Dictionary<string, object>
        {
            { "time", "1733011200" } // Unix timestamp for 2023-01-01T12:00:00
        };

            // Act
            story.ApplyFirebaseUpdate(updateData);

            // Assert
            var firstDecember2024 = new DateTime(2024, 12, 1, 0, 0, 0);
            Assert.Equal(firstDecember2024, DateTime.Parse(story.Time)); // Adjust format if necessary for your locale
        }

        [Fact]
        public void ApplyFirebaseUpdate_ShouldUpdateCommentCount()
        {
            // Arrange
            var story = new Story
            {
                Id = 1,
                CommentCount = 5
            };

            var updateData = new Dictionary<string, object>
        {
            { "descendants", "10" }
        };

            // Act
            story.ApplyFirebaseUpdate(updateData);

            // Assert
            Assert.Equal(10, story.CommentCount);
        }

        [Fact]
        public void ApplyFirebaseUpdate_ShouldNotUpdateUnknownFields()
        {
            // Arrange
            var story = new Story
            {
                Id = 1,
                Title = "Original Title",
                Uri = "https://originaluri.com"
            };

            var updateData = new Dictionary<string, object>
        {
            { "unknown_field", "Some Value" }
        };

            // Act
            story.ApplyFirebaseUpdate(updateData);

            // Assert
            Assert.Equal("Original Title", story.Title);
            Assert.Equal("https://originaluri.com", story.Uri);
        }
    }
}

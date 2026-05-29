using AzureDevOpsToPowerBI.Models.Dtos;
using Xunit;

namespace AzureDevOpsToPowerBI.Tests
{
    public class BugDtoTests
    {
        [Fact]
        public void ToEntity_MapsStoryPointsCorrectly()
        {
            // Arrange
            var dto = new BugDto
            {
                WorkItemId = 12345,
                Title = "Critical crash bug",
                State = "Active",
                Severity = "1 - Critical",
                StoryPoints = 8.5,
                CreatedDate = "2026-05-28T00:00:00Z",
                CompletedDate = "2026-05-28T00:00:00Z",
                ActivatedDate = "2026-05-28T00:00:00Z",
                ResolvedDate = "2026-05-28T00:00:00Z",
                ClosedDate = "2026-05-28T00:00:00Z"
            };

            // Act
            var entity = dto.ToEntity("TestProjectKey");

            // Assert
            Assert.Equal(12345, entity.WorkItemId);
            Assert.Equal("TestProjectKey", entity.ProjectKey);
            Assert.Equal("Critical crash bug", entity.Title);
            Assert.Equal("Active", entity.State);
            Assert.Equal("1 - Critical", entity.Severity);
            Assert.Equal(8.5, entity.StoryPoints);
        }

        [Fact]
        public void ToEntity_WithNullStoryPoints_MapsNullCorrectly()
        {
            // Arrange
            var dto = new BugDto
            {
                WorkItemId = 12345,
                Title = "Minor visual bug",
                State = "New",
                Severity = "3 - Medium",
                StoryPoints = null,
                CreatedDate = "2026-05-28T00:00:00Z",
                CompletedDate = "2026-05-28T00:00:00Z",
                ActivatedDate = "2026-05-28T00:00:00Z",
                ResolvedDate = "2026-05-28T00:00:00Z",
                ClosedDate = "2026-05-28T00:00:00Z"
            };

            // Act
            var entity = dto.ToEntity("TestProjectKey");

            // Assert
            Assert.Null(entity.StoryPoints);
        }
    }
}

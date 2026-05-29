using AzureDevOpsToPowerBI.Models.Dtos;
using Xunit;

namespace AzureDevOpsToPowerBI.Tests
{
    public class FeatureDtoTests
    {
        [Fact]
        public void ToEntity_MapsStoryPointsCorrectly()
        {
            // Arrange
            var dto = new FeatureDto
            {
                WorkItemId = 98765,
                Title = "Implement dark mode",
                State = "Active",
                StoryPoints = 13.0,
                CreatedDate = "2026-05-28T00:00:00Z",
                CompletedDate = "2026-05-28T00:00:00Z",
                ActivatedDate = "2026-05-28T00:00:00Z",
                ResolvedDate = "2026-05-28T00:00:00Z",
                ClosedDate = "2026-05-28T00:00:00Z"
            };

            // Act
            var entity = dto.ToEntity("TestProjectKey");

            // Assert
            Assert.Equal(98765, entity.WorkItemId);
            Assert.Equal("TestProjectKey", entity.ProjectKey);
            Assert.Equal("Implement dark mode", entity.Title);
            Assert.Equal("Active", entity.State);
            Assert.Equal(13.0, entity.StoryPoints);
        }

        [Fact]
        public void ToEntity_WithNullStoryPoints_MapsNullCorrectly()
        {
            // Arrange
            var dto = new FeatureDto
            {
                WorkItemId = 98765,
                Title = "Refactor auth pipeline",
                State = "New",
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

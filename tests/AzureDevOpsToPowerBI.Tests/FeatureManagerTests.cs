using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AzureDevOpsToPowerBI.Manager;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace AzureDevOpsToPowerBI.Tests
{
    public class FeatureManagerTests
    {
        [Fact]
        public async Task GetTfsFeaturesAsync_FetchesAndDeserializesFeaturesWithStoryPoints()
        {
            // Arrange
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockLogger = new Mock<ILogger<FeatureManager>>();

            var fakeODataResponse = new
            {
                value = new[]
                {
                    new
                    {
                        WorkItemId = 201,
                        Title = "Implement dark theme",
                        State = "In Progress",
                        AreaSK = "AreaA",
                        IterationSK = "IterB",
                        CreatedDate = "2026-05-01T12:00:00Z",
                        ActivatedDate = "2026-05-02T12:00:00Z",
                        ClosedDate = "2026-05-05T12:00:00Z",
                        ResolvedDate = "2026-05-04T12:00:00Z",
                        CompletedDate = "2026-05-05T12:00:00Z",
                        StoryPoints = (double?)13.0
                    },
                    new
                    {
                        WorkItemId = 202,
                        Title = "Security enhancements",
                        State = "New",
                        AreaSK = "AreaA",
                        IterationSK = "IterB",
                        CreatedDate = "2026-05-01T13:00:00Z",
                        ActivatedDate = "2026-05-01T13:00:00Z",
                        ClosedDate = "2026-05-01T13:00:00Z",
                        ResolvedDate = "2026-05-01T13:00:00Z",
                        CompletedDate = "2026-05-01T13:00:00Z",
                        StoryPoints = (double?)null
                    }
                }
            };

            var jsonResponse = JsonSerializer.Serialize(fakeODataResponse);

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var manager = new FeatureManager(mockFactory.Object, mockLogger.Object);

            // Act
            var result = await manager.GetTfsFeaturesAsync(
                tfsUri: "https://fake-tfs.com",
                pat: "fakepat123",
                projectKey: "TestKey",
                projectName: "TestProj",
                areaPath: "AreaPath1",
                syncDate: "2026-05-01T00:00:00Z",
                tags: new System.Collections.Generic.List<string>()
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var feat1 = result[0];
            Assert.Equal(201, feat1.WorkItemId);
            Assert.Equal("Implement dark theme", feat1.Title);
            Assert.Equal(13.0, feat1.StoryPoints);

            var feat2 = result[1];
            Assert.Equal(202, feat2.WorkItemId);
            Assert.Equal("Security enhancements", feat2.Title);
            Assert.Null(feat2.StoryPoints);

            // Verify that the requested OData query contains StoryPoints in $Select and WorkItemType eq 'Feature'
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.Query.Contains("$select=") &&
                    req.RequestUri.Query.Contains("StoryPoints") &&
                    (req.RequestUri.Query.Contains("WorkItemType%20eq%20'Feature'") || req.RequestUri.Query.Contains("WorkItemType eq 'Feature'"))),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}

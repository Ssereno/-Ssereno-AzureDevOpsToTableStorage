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
    public class BugManagerTests
    {
        [Fact]
        public async Task GetBugsAsync_FetchesAndDeserializesBugsWithStoryPoints()
        {
            // Arrange
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockLogger = new Mock<ILogger<BugManager>>();

            var fakeODataResponse = new
            {
                value = new[]
                {
                    new
                    {
                        WorkItemId = 101,
                        Title = "Performance slowdown",
                        State = "Proposed",
                        AreaSK = "Area123",
                        IterationSK = "Iter456",
                        CreatedDate = "2026-05-01T12:00:00Z",
                        ActivatedDate = "2026-05-02T12:00:00Z",
                        ClosedDate = "2026-05-05T12:00:00Z",
                        ResolvedDate = "2026-05-04T12:00:00Z",
                        CompletedDate = "2026-05-05T12:00:00Z",
                        Severity = "2 - High",
                        StoryPoints = (double?)5.0
                    },
                    new
                    {
                        WorkItemId = 102,
                        Title = "Typo in home screen",
                        State = "New",
                        AreaSK = "Area123",
                        IterationSK = "Iter456",
                        CreatedDate = "2026-05-01T13:00:00Z",
                        ActivatedDate = "2026-05-01T13:00:00Z",
                        ClosedDate = "2026-05-01T13:00:00Z",
                        ResolvedDate = "2026-05-01T13:00:00Z",
                        CompletedDate = "2026-05-01T13:00:00Z",
                        Severity = "4 - Low",
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

            var manager = new BugManager(mockFactory.Object, mockLogger.Object);

            // Act
            var result = await manager.GetBugsAsync(
                tfsUri: "https://fake-tfs.com",
                pat: "fakepat123",
                projectKey: "TestKey",
                projectName: "TestProj",
                areaPath: "AreaPath1",
                syncDate: "2026-05-01T00:00:00Z"
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var bug1 = result[0];
            Assert.Equal(101, bug1.WorkItemId);
            Assert.Equal("Performance slowdown", bug1.Title);
            Assert.Equal(5.0, bug1.StoryPoints);

            var bug2 = result[1];
            Assert.Equal(102, bug2.WorkItemId);
            Assert.Equal("Typo in home screen", bug2.Title);
            Assert.Null(bug2.StoryPoints);

            // Verify that the requested OData query contains StoryPoints in $Select
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.Query.Contains("$Select=") &&
                    req.RequestUri.Query.Contains("StoryPoints")),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}

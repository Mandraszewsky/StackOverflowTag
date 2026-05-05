using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using StackOverflow.Application.Tags.Dtos;
using StackOverflow.Domain.Entities;
using StackOverflow.IntegrationTests.Infrastructure;

namespace StackOverflow.IntegrationTests.Tags;

public class RefreshTagsEndpointTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task RefreshTags_ShouldReturn204NoContent()
    {
        // Act
        var response = await Client.PostAsync("/api/tags/refresh", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RefreshTags_ShouldReplaceExistingTags()
    {
        // Arrange — seed old tags
        var oldTags = new List<Tag>
        {
            new() { Name = "old-tag-1", Count = 10, Percentage = 50.0, FetchedAt = DateTime.UtcNow.AddDays(-1) },
            new() { Name = "old-tag-2", Count = 10, Percentage = 50.0, FetchedAt = DateTime.UtcNow.AddDays(-1) }
        };
        await SeedTagsAsync(oldTags);

        // Configure mock to return new tags
        var newTags = new List<Tag>
        {
            new() { Name = "new-tag-1", Count = 300 },
            new() { Name = "new-tag-2", Count = 500 },
            new() { Name = "new-tag-3", Count = 200 }
        };
        Factory.ApiServiceMock
            .Setup(s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTags);

        // Act
        var refreshResponse = await Client.PostAsync("/api/tags/refresh", null);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert — GET tags should return only new tags
        var getResponse = await Client.GetAsync("/api/tags?sortBy=name&sortDirection=asc");
        var result = await getResponse.Content.ReadFromJsonAsync<PagedResultDto<TagResponseDto>>();

        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(3);

        var names = result.Items.Select(t => t.Name).ToList();
        names.Should().Contain("new-tag-1");
        names.Should().Contain("new-tag-2");
        names.Should().Contain("new-tag-3");
        names.Should().NotContain("old-tag-1");
        names.Should().NotContain("old-tag-2");
    }
}

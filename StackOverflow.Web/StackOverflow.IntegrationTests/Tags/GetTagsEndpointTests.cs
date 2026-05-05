using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StackOverflow.Application.Tags.Dtos;
using StackOverflow.Domain.Entities;
using StackOverflow.IntegrationTests.Infrastructure;

namespace StackOverflow.IntegrationTests.Tags;

public class GetTagsEndpointTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetTags_WithDefaultParams_ShouldReturnPaginatedResult()
    {
        // Arrange
        var tags = GenerateTags(30);
        await SeedTagsAsync(tags);

        // Act
        var response = await Client.GetAsync("/api/tags");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<TagResponseDto>>();
        result.Should().NotBeNull();
        result!.Items.Count().Should().Be(25);
        result.TotalCount.Should().Be(30);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(25);
    }

    [Fact]
    public async Task GetTags_WithCustomPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var tags = GenerateTags(30);
        await SeedTagsAsync(tags);

        // Act
        var response = await Client.GetAsync("/api/tags?pageNumber=2&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<TagResponseDto>>();
        result.Should().NotBeNull();
        result!.Items.Count().Should().Be(10);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(30);
    }

    [Fact]
    public async Task GetTags_SortByNameDesc_ShouldReturnDescendingOrder()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Name = "alpha", Count = 100, Percentage = 33.3, FetchedAt = DateTime.UtcNow },
            new() { Name = "beta", Count = 200, Percentage = 33.3, FetchedAt = DateTime.UtcNow },
            new() { Name = "gamma", Count = 300, Percentage = 33.3, FetchedAt = DateTime.UtcNow }
        };
        await SeedTagsAsync(tags);

        // Act
        var response = await Client.GetAsync("/api/tags?sortBy=name&sortDirection=desc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<TagResponseDto>>();
        result.Should().NotBeNull();

        var names = result!.Items.Select(t => t.Name).ToList();
        names[0].Should().Be("gamma");
        names[1].Should().Be("beta");
        names[2].Should().Be("alpha");
    }

    [Fact]
    public async Task GetTags_SortByCountAsc_ShouldReturnAscendingOrder()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Name = "big", Count = 300, Percentage = 50.0, FetchedAt = DateTime.UtcNow },
            new() { Name = "small", Count = 100, Percentage = 16.7, FetchedAt = DateTime.UtcNow },
            new() { Name = "medium", Count = 200, Percentage = 33.3, FetchedAt = DateTime.UtcNow }
        };
        await SeedTagsAsync(tags);

        // Act
        var response = await Client.GetAsync("/api/tags?sortBy=count&sortDirection=asc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<TagResponseDto>>();
        result.Should().NotBeNull();

        var counts = result!.Items.Select(t => t.Count).ToList();
        counts.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetTags_WhenNoTags_ShouldReturnEmptyResult()
    {
        // Act
        var response = await Client.GetAsync("/api/tags");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<TagResponseDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}

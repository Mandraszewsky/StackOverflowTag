using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StackOverflow.Domain.Entities;
using StackOverflow.Infrastructure.Data;
using StackOverflow.Infrastructure.Repositories;

namespace StackOverflow.UnitTests.Infrastructure.Repositories;

public class TagRepositoryApplySortingTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TagRepository _sut;

    public TagRepositoryApplySortingTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _sut = new TagRepository(_dbContext);

        SeedData();
    }

    private void SeedData()
    {
        _dbContext.Tags.AddRange(
            new Tag { Name = "c#", Count = 300, Percentage = 30.0, FetchedAt = DateTime.UtcNow },
            new Tag { Name = "java", Count = 100, Percentage = 10.0, FetchedAt = DateTime.UtcNow },
            new Tag { Name = "python", Count = 200, Percentage = 20.0, FetchedAt = DateTime.UtcNow }
        );
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetPagedAsync_SortByName_Asc_ShouldReturnAlphabeticalOrder()
    {
        // Act
        var (items, _) = await _sut.GetPagedAsync(1, 10, "name", "asc");

        // Assert
        var names = items.Select(t => t.Name).ToList();
        names.Should().BeInAscendingOrder();
        names.Should().ContainInOrder("c#", "java", "python");
    }

    [Fact]
    public async Task GetPagedAsync_SortByName_Desc_ShouldReturnReverseAlphabeticalOrder()
    {
        // Act
        var (items, _) = await _sut.GetPagedAsync(1, 10, "name", "desc");

        // Assert
        var names = items.Select(t => t.Name).ToList();
        names.Should().BeInDescendingOrder();
        names.Should().ContainInOrder("python", "java", "c#");
    }

    [Fact]
    public async Task GetPagedAsync_SortByCount_Asc_ShouldReturnLowestFirst()
    {
        // Act
        var (items, _) = await _sut.GetPagedAsync(1, 10, "count", "asc");

        // Assert
        var counts = items.Select(t => t.Count).ToList();
        counts.Should().BeInAscendingOrder();
        counts.Should().ContainInOrder(100L, 200L, 300L);
    }

    [Fact]
    public async Task GetPagedAsync_SortByCount_Desc_ShouldReturnHighestFirst()
    {
        // Act
        var (items, _) = await _sut.GetPagedAsync(1, 10, "count", "desc");

        // Assert
        var counts = items.Select(t => t.Count).ToList();
        counts.Should().BeInDescendingOrder();
        counts.Should().ContainInOrder(300L, 200L, 100L);
    }

    [Fact]
    public async Task GetPagedAsync_SortByPercentage_Asc_ShouldReturnLowestFirst()
    {
        // Act
        var (items, _) = await _sut.GetPagedAsync(1, 10, "percentage", "asc");

        // Assert
        var percentages = items.Select(t => t.Percentage).ToList();
        percentages.Should().BeInAscendingOrder();
        percentages.Should().ContainInOrder(10.0, 20.0, 30.0);
    }

    [Fact]
    public async Task GetPagedAsync_SortByUnknownField_ShouldDefaultToCount()
    {
        // Act
        var (items, _) = await _sut.GetPagedAsync(1, 10, "unknown_field", "asc");

        // Assert — default sorting is by Count (ascending)
        var counts = items.Select(t => t.Count).ToList();
        counts.Should().BeInAscendingOrder();
        counts.Should().ContainInOrder(100L, 200L, 300L);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}

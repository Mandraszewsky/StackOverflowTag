using FluentAssertions;
using MediatR;
using Moq;
using StackOverflow.Application.Abstractions;
using StackOverflow.Application.Tags.Commands;
using StackOverflow.Domain.Entities;

namespace StackOverflow.UnitTests.Tags.Commands;

public class RefreshTagsCommandHandlerTests
{
    private readonly Mock<IStackOverflowApiService> _apiServiceMock = new();
    private readonly Mock<ITagRepository> _tagRepositoryMock = new();
    private readonly RefreshTagsCommandHandler _sut;

    public RefreshTagsCommandHandlerTests()
    {
        _sut = new RefreshTagsCommandHandler(_apiServiceMock.Object, _tagRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAlwaysFetchTagsFromApi()
    {
        // Arrange
        _apiServiceMock.Setup(s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Tag { Name = "c#", Count = 100 }]);

        // Act
        await _sut.Handle(new RefreshTagsCommand(), CancellationToken.None);

        // Assert
        _apiServiceMock.Verify(
            s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReplaceAllTagsInRepository()
    {
        // Arrange
        var tags = new List<Tag> { new() { Name = "c#", Count = 100 } };
        _apiServiceMock.Setup(s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        // Act
        await _sut.Handle(new RefreshTagsCommand(), CancellationToken.None);

        // Assert
        _tagRepositoryMock.Verify(
            r => r.ReplaceAllAsync(It.IsAny<IEnumerable<Tag>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCalculatePercentagesCorrectly()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Name = "c#", Count = 100 },
            new() { Name = "java", Count = 200 },
            new() { Name = "python", Count = 700 }
        };

        _apiServiceMock.Setup(s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        List<Tag>? savedTags = null;
        _tagRepositoryMock.Setup(r => r.ReplaceAllAsync(It.IsAny<IEnumerable<Tag>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Tag>, CancellationToken>((t, _) => savedTags = t.ToList());

        // Act
        await _sut.Handle(new RefreshTagsCommand(), CancellationToken.None);

        // Assert
        savedTags.Should().NotBeNull();
        savedTags.Should().HaveCount(3);
        savedTags![0].Percentage.Should().Be(10.0);
        savedTags[1].Percentage.Should().Be(20.0);
        savedTags[2].Percentage.Should().Be(70.0);
    }

    [Fact]
    public async Task Handle_ShouldSetFetchedAtToUtcNow()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Name = "c#", Count = 100 },
            new() { Name = "java", Count = 200 }
        };

        _apiServiceMock.Setup(s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        List<Tag>? savedTags = null;
        _tagRepositoryMock.Setup(r => r.ReplaceAllAsync(It.IsAny<IEnumerable<Tag>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Tag>, CancellationToken>((t, _) => savedTags = t.ToList());

        var beforeExecution = DateTime.UtcNow;

        // Act
        await _sut.Handle(new RefreshTagsCommand(), CancellationToken.None);

        var afterExecution = DateTime.UtcNow;

        // Assert
        savedTags.Should().NotBeNull();
        savedTags!.Should().AllSatisfy(t =>
        {
            t.FetchedAt.Should().BeOnOrAfter(beforeExecution);
            t.FetchedAt.Should().BeOnOrBefore(afterExecution);
        });
    }
}

using FluentAssertions;
using MediatR;
using Moq;
using StackOverflow.Application.Abstractions;
using StackOverflow.Application.Tags.Commands;
using StackOverflow.Domain.Entities;

namespace StackOverflow.UnitTests.Tags.Commands;

public class FetchTagsCommandHandlerTests
{
    private readonly Mock<IStackOverflowApiService> _apiServiceMock = new();
    private readonly Mock<ITagRepository> _tagRepositoryMock = new();
    private readonly FetchTagsCommandHandler _sut;

    public FetchTagsCommandHandlerTests()
    {
        _sut = new FetchTagsCommandHandler(_apiServiceMock.Object, _tagRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenTagsExistInDatabase_ShouldNotFetchFromApi()
    {
        // Arrange
        _tagRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _sut.Handle(new FetchTagsCommand(), CancellationToken.None);

        // Assert
        _apiServiceMock.Verify(
            s => s.FetchTagsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _tagRepositoryMock.Verify(
            r => r.ReplaceAllAsync(It.IsAny<IEnumerable<Tag>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDatabaseEmpty_ShouldFetchAndSaveTags()
    {
        // Arrange
        _tagRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _apiServiceMock.Setup(s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Tag { Name = "c#", Count = 100 }]);

        // Act
        await _sut.Handle(new FetchTagsCommand(), CancellationToken.None);

        // Assert
        _apiServiceMock.Verify(
            s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()),
            Times.Once);
        _tagRepositoryMock.Verify(
            r => r.ReplaceAllAsync(It.IsAny<IEnumerable<Tag>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDatabaseEmpty_ShouldCalculatePercentagesCorrectly()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Name = "c#", Count = 50 },
            new() { Name = "java", Count = 30 },
            new() { Name = "python", Count = 20 }
        };

        _tagRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _apiServiceMock.Setup(s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        List<Tag>? savedTags = null;
        _tagRepositoryMock.Setup(r => r.ReplaceAllAsync(It.IsAny<IEnumerable<Tag>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Tag>, CancellationToken>((t, _) => savedTags = t.ToList());

        // Act
        await _sut.Handle(new FetchTagsCommand(), CancellationToken.None);

        // Assert
        savedTags.Should().NotBeNull();
        savedTags.Should().HaveCount(3);
        savedTags![0].Percentage.Should().Be(50.0);
        savedTags[1].Percentage.Should().Be(30.0);
        savedTags[2].Percentage.Should().Be(20.0);
    }

    [Fact]
    public async Task Handle_WhenTotalCountIsZero_ShouldSetPercentagesToZero()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Name = "empty1", Count = 0 },
            new() { Name = "empty2", Count = 0 }
        };

        _tagRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _apiServiceMock.Setup(s => s.FetchTagsAsync(1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        List<Tag>? savedTags = null;
        _tagRepositoryMock.Setup(r => r.ReplaceAllAsync(It.IsAny<IEnumerable<Tag>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Tag>, CancellationToken>((t, _) => savedTags = t.ToList());

        // Act
        await _sut.Handle(new FetchTagsCommand(), CancellationToken.None);

        // Assert
        savedTags.Should().NotBeNull();
        savedTags!.Should().AllSatisfy(t => t.Percentage.Should().Be(0));
    }
}

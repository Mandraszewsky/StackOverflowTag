using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StackOverflow.Application.Abstractions;
using StackOverflow.Application.Mappings;
using StackOverflow.Application.Tags.Dtos;
using StackOverflow.Application.Tags.Queries;
using StackOverflow.Domain.Entities;

namespace StackOverflow.UnitTests.Tags.Queries;

public class GetTagsQueryHandlerTests
{
    private readonly Mock<ITagRepository> _tagRepositoryMock = new();
    private readonly IMapper _mapper;
    private readonly GetTagsQueryHandler _sut;

    public GetTagsQueryHandlerTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
        _sut = new GetTagsQueryHandler(_tagRepositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        var query = new GetTagsQuery
        {
            PageNumber = 2,
            PageSize = 10,
            SortBy = "name",
            SortDirection = "desc"
        };

        _tagRepositoryMock.Setup(r => r.GetPagedAsync(2, 10, "name", "desc", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enumerable.Empty<Tag>(), 0));

        // Act
        await _sut.Handle(query, CancellationToken.None);

        // Assert
        _tagRepositoryMock.Verify(
            r => r.GetPagedAsync(2, 10, "name", "desc", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapTagsToResponseDtos()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Id = 1, Name = "c#", Count = 500, Percentage = 50.0 },
            new() { Id = 2, Name = "java", Count = 300, Percentage = 30.0 }
        };

        _tagRepositoryMock.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((tags.AsEnumerable(), 2));

        var query = new GetTagsQuery { PageNumber = 1, PageSize = 25 };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        var items = result.Items.ToList();
        items.Should().HaveCount(2);
        items[0].Name.Should().Be("c#");
        items[0].Count.Should().Be(500);
        items[0].Percentage.Should().Be(50.0);
        items[1].Name.Should().Be("java");
        items[1].Count.Should().Be(300);
        items[1].Percentage.Should().Be(30.0);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectPaginationMetadata()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Id = 1, Name = "c#", Count = 500, Percentage = 50.0 }
        };

        _tagRepositoryMock.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((tags.AsEnumerable(), 100));

        var query = new GetTagsQuery { PageNumber = 3, PageSize = 15 };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(100);
        result.PageNumber.Should().Be(3);
        result.PageSize.Should().Be(15);
    }
}

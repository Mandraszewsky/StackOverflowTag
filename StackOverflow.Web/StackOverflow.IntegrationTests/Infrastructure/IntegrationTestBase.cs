using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackOverflow.Application.Abstractions;
using StackOverflow.Domain.Entities;
using StackOverflow.Infrastructure.Data;

namespace StackOverflow.IntegrationTests.Infrastructure;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await CleanDatabaseAsync();

        Factory.ApiServiceMock.Reset();
        Factory.ApiServiceMock
            .Setup(s => s.FetchTagsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tag>());
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task SeedTagsAsync(IEnumerable<Tag> tags)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Tags.AddRange(tags);
        await dbContext.SaveChangesAsync();
    }

    protected async Task CleanDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Tags.RemoveRange(dbContext.Tags);
        await dbContext.SaveChangesAsync();
    }

    protected static List<Tag> GenerateTags(int count)
    {
        return Enumerable.Range(1, count).Select(i => new Tag
        {
            Name = $"tag-{i:D3}",
            Count = i * 10,
            Percentage = (double)i / count * 100,
            FetchedAt = DateTime.UtcNow
        }).ToList();
    }
}

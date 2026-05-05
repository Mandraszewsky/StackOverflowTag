using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using StackOverflow.Application.Abstractions;
using StackOverflow.Domain.Entities;
using StackOverflow.Infrastructure.Data;
using Testcontainers.MsSql;

namespace StackOverflow.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private readonly Mock<IStackOverflowApiService> _apiServiceMock = new();

    public Mock<IStackOverflowApiService> ApiServiceMock => _apiServiceMock;

    public async Task InitializeAsync()
    {
        // Default: return empty list so startup FetchTagsCommand does nothing
        _apiServiceMock
            .Setup(s => s.FetchTagsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tag>());

        await _dbContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Replace DbContext with Testcontainers SQL Server
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));

            // Replace external API service with mock
            services.RemoveAll<IStackOverflowApiService>();
            services.AddSingleton(_apiServiceMock.Object);
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}

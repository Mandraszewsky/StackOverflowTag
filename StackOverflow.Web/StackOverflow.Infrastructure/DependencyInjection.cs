using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackOverflow.Application.Abstractions;
using StackOverflow.Infrastructure.Data;
using StackOverflow.Infrastructure.ExternalServices.StackOverflowApiService.Settings;
using StackOverflow.Infrastructure.Repositories;
using StackOverflowApiServiceImpl = StackOverflow.Infrastructure.ExternalServices.StackOverflowApiService.Services.StackOverflowApiService;

namespace StackOverflow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<StackOverflowApiSettings>(
            configuration.GetSection(StackOverflowApiSettings.SectionName));

        services.AddHttpClient<IStackOverflowApiService, StackOverflowApiServiceImpl>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("User-Agent", "StackOverflowTagsApp/1.0");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

        services.AddScoped<ITagRepository, TagRepository>();

        return services;
    }
}

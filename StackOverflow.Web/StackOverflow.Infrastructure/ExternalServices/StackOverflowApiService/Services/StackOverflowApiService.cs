using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackOverflow.Application.Abstractions;
using StackOverflow.Domain.Entities;
using StackOverflow.Infrastructure.ExternalServices.StackOverflowApiService.Dtos;
using StackOverflow.Infrastructure.ExternalServices.StackOverflowApiService.Settings;

namespace StackOverflow.Infrastructure.ExternalServices.StackOverflowApiService.Services;

public class StackOverflowApiService(
    HttpClient httpClient,
    IOptions<StackOverflowApiSettings> settings,
    ILogger<StackOverflowApiService> logger) : IStackOverflowApiService
{
    private readonly StackOverflowApiSettings _settings = settings.Value;

    public async Task<List<Tag>> FetchTagsAsync(int minCount = 1000, CancellationToken cancellationToken = default)
    {
        var tags = new List<Tag>();
        var page = 1;
        var hasMore = true;

        logger.LogInformation("Starting to fetch tags from StackOverflow API. Target: {MinCount} tags", minCount);

        while (hasMore && tags.Count < minCount)
        {
            var url = BuildUrl(page);

            logger.LogDebug("Fetching page {Page} from StackOverflow API", page);

            var response = await httpClient.GetFromJsonAsync<StackOverflowApiResponseDto<StackOverflowTagItem>>(
                url, cancellationToken);

            if (response is null || response.Items.Count == 0)
                break;

            foreach (var item in response.Items)
            {
                tags.Add(new Tag
                {
                    Name = item.Name,
                    Count = item.Count
                });
            }

            hasMore = response.HasMore;
            page++;

            logger.LogDebug("Fetched {Count} tags so far. Has more: {HasMore}, Quota remaining: {Quota}",
                tags.Count, hasMore, response.QuotaRemaining);

            if (response.QuotaRemaining <= 0)
            {
                logger.LogWarning("StackOverflow API quota exhausted. Collected {Count} tags", tags.Count);
                break;
            }
        }

        logger.LogInformation("Finished fetching tags. Total collected: {Count}", tags.Count);

        return tags;
    }

    private string BuildUrl(int page)
    {
        var url = $"{_settings.BaseUrl}/tags?order=desc&sort=popular&site=stackoverflow&page={page}&pagesize={_settings.PageSize}";

        if (!string.IsNullOrEmpty(_settings.ApiKey))
            url += $"&key={_settings.ApiKey}";

        return url;
    }
}
